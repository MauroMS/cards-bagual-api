using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace ShitheadCardsApi
{
    public class GameService : IGameService
    {
        private static ConcurrentDictionary<string, object> locker = new ConcurrentDictionary<string, object>();


        private ShitheadDBContext _ctx;
        private IShitheadService _shitheadService;

        public GameService(ShitheadDBContext ctx, IShitheadService shitheadService)
        {
            _ctx = ctx;
            _shitheadService = shitheadService;
        }

        public Game CreateOrJoinGame(string gameName, string playerName)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);

                if (game == null)
                {
                    game = CreateGame(gameName);
                }

                game = JoinGame(game, playerName);

                SaveGame(game);

                return game;
            }
        }

        private Game CreateGame(string gameName)
        {
            List<string> cards = _shitheadService.CreateDeck();

            var game = new Game
            {
                Name = gameName,
                Status = StatusEnum.SETUP,
                CardsInDeck = cards,
                BurnedCardsCount = 0,
                LastBurnedCard = null,
                PlayerIdTurn = null,
                TableCards = { },
                DateCreated = DateTime.Now
            };

            return game;
        }

        public Game JoinGame(Game game, string playerName)
        {
            if (game.Status != StatusEnum.SETUP)
            {
                throw new GameException("Game not in setup mode: " + game.Status);
            }

            var player = game.Players.FirstOrDefault(player1 => player1.Name.Equals(playerName));

            if (player == null)
            {
                if (game.Players.Count == 5)
                {
                    throw new GameException("Max number of players reached: 5");
                }

                List<string> playerCards = game.CardsInDeck.GetRange(0, 9);
                game.CardsInDeck.RemoveRange(0, 9);
                player = new Player
                {
                    Id = new string(Guid.NewGuid().ToString().TakeLast(12).ToArray()),
                    Name = playerName,
                    Status = StatusEnum.SETUP,
                    DownCards = playerCards.GetRange(0, 3),
                    OpenCards = playerCards.GetRange(3, 3),
                    InHandCards = playerCards.GetRange(6, 3),
                };
                game.Players.Add(player);
            }

            return game;
        }


        public Game GetGame(string name)
        {
            GameDbModel gameDbModel = _ctx.Find<GameDbModel>(name);
            if (gameDbModel == null)
                return null;
            return Deserialize(gameDbModel);
        }

        public Game SwitchPlayerCards(string gameName, string playerId, string openCard, string handCard)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);
                ValidateGame(game, StatusEnum.SETUP);

                Player player = game.Players.FirstOrDefault(p => p.Id == playerId);
                ValidatePlayer(player, StatusEnum.SETUP);

                if (player.InHandCards.Remove(handCard))
                    throw new GameException("Player hand card not found: " + handCard);

                if (player.OpenCards.Remove(openCard))
                    throw new GameException("Player open card not found: " + openCard);

                player.OpenCards.Add(handCard);
                player.InHandCards.Add(openCard);

                SaveGame(game);

                return game;
            }
        }

        public Game SetPlayerToStart(string gameName, string playerId)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);
                ValidateGame(game, StatusEnum.SETUP);

                Player player = game.Players.FirstOrDefault(p => p.Id == playerId);
                ValidatePlayer(player, StatusEnum.SETUP);

                player.Status = StatusEnum.PLAYING;

                if (!game.Players.Any(p => p.Status != StatusEnum.PLAYING))
                {
                    game.Status = StatusEnum.PLAYING;
                    game.PlayerIdTurn = _shitheadService.ChooseFirstTurn(game.Players);
                }

                SaveGame(game);

                return game;
            }
        }

        public Game MoveTableCardsToPlayer(string gameName, string playerId)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);
                ValidateGame(game, StatusEnum.PLAYING);

                Player player = game.Players.FirstOrDefault(p => p.Id == playerId);
                ValidatePlayer(player, StatusEnum.PLAYING, game.PlayerIdTurn);

                player.InHandCards.AddRange(game.TableCards);
                game.TableCards.Clear();

                game.PlayerIdTurn = _shitheadService.NextPlayerFrom(game.Players, playerId);

                SaveGame(game);

                return game;
            }
        }

        public Game DiscardPlayerCards(string gameName, string playerId, string cards)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);
                ValidateGame(game, StatusEnum.PLAYING);

                Player player = game.Players.FirstOrDefault(player => player.Id.Equals(playerId));
                ValidatePlayer(player, StatusEnum.PLAYING, game.PlayerIdTurn);




                player.InHandCards.Count();

                List<string> movedCards = cards.Split(",").ToList();
                game.TableCards.AddRange(movedCards);
                return game;
            }
        }

        private static object GetGameLock(string gameName)
        {
            return locker.GetOrAdd(gameName, new Object());
        }

        private void SaveGame(Game game)
        {
            GameDbModel gameDbModel = _ctx.Find<GameDbModel>(game.Name);

            if (gameDbModel == null)
            {
                gameDbModel = new GameDbModel()
                {
                    Name = game.Name,
                    GameJson = Serialize(game)
                };
                _ctx.ShitheadGames.Add(gameDbModel);
            }
            else
            {
                gameDbModel.GameJson = Serialize(game);
            }

            _ctx.SaveChanges();
        }

        private string Serialize(Game game)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(game, options);
        }

        private Game Deserialize(GameDbModel game)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return JsonSerializer.Deserialize<Game>(game.GameJson, options);
        }

        private void ValidatePlayer(Player player, StatusEnum expectedStatus, string playerIdTurn = null)
        {
            if (player == null)
                throw new GameException("Player not found");

            if (player.Status != expectedStatus)
                throw new GameException($"Player not in {expectedStatus} mode: {player.Status}");

            if (!String.IsNullOrEmpty(playerIdTurn) && player.Name != playerIdTurn)
                throw new GameException("Player not in turn");
        }

        private void ValidateGame(Game game, StatusEnum expectedStatus)
        {
            if (game == null)
                throw new GameException("Game not found");

            if (game.Status != expectedStatus)
                throw new GameException($"Game not in {expectedStatus} mode: {game.Status}");

        }
    }
}
