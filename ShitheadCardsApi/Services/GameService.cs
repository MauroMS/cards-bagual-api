using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShitheadCardsApi
{
    public class GameService : IGameService
    {
        private static ConcurrentDictionary<string, object> locker = new ConcurrentDictionary<string, object>();


        private ShitheadDBContext _ctx;
        private IShitheadService shitheadService;
        public GameService(ShitheadDBContext ctx, IShitheadService shitheadService)
        {
            _ctx = ctx;
            this.shitheadService = shitheadService;
        }

        public Game CreateOrJoinGame(string gameName, string playerName)
        {
            object gameLock = GetGameLock(gameName);

            lock (gameLock)
            {
                Game game = GetGame(gameName);

                if (game == null)
                {
                    game = CreateGame(gameName, playerName);
                } 

                game = JoinGame(game, playerName);

                SaveGame(game);

                return game;
            }
        }

        private Game CreateGame(string gameName, string playerName)
        {
            List<string> cards = shitheadService.CreateDeck();

            var game = new Game
            {
                Name = gameName,
                Status = StatusEnum.SETUP,
                CardsInDeck = cards,
                BurnedCardsCount = 0,
                LastBurnedCard = null,
                PlayerNameTurn = null,
                TableCards = { }
            };

            return game;
        }

        public Game JoinGame(Game game, string playerName)
        {
            if (game.Status != StatusEnum.SETUP)
            {
                throw new GameException("Game not in setup mode: " + game.Status);
            }

            var player = game.Players.Find(player1 => player1.Name.Equals(playerName));

            if (player == null)
            {
                if (game.Players.Count == 5)
                {
                    throw new GameException("Max number of players reached: 5");
                }
                
                List<string> playerCards = game.CardsInDeck.GetRange(0,9);
                game.CardsInDeck.RemoveRange(0, 9);
                player = new Player
                {
                    Id = new string(Guid.NewGuid().ToString().TakeLast(12).ToArray()),
                    Name = playerName,
                    Status = StatusEnum.SETUP,
                    DownCards = playerCards.GetRange(0,3),
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

                if (game == null)
                    throw new GameException("Game not found");

                if (game.Status != StatusEnum.SETUP)
                    throw new GameException("Game not in setup mode: " + game.Status);
                
                Player player = game.Players.Find(p => p.Id == playerId);

                if (player == null)
                    throw new GameException("Player not found");

                if (player.Status != StatusEnum.SETUP)
                    throw new GameException("Player not in setup mode: " + player.Status);
                
                if (! player.InHandCards.Remove(handCard))
                    throw new GameException("Player hand card not found: " + handCard);

                if (! player.OpenCards.Remove(openCard))
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

                if (game == null)
                    throw new GameException("Game not found");

                if (game.Status != StatusEnum.SETUP)
                    throw new GameException("Game not in setup mode: " + game.Status);

                Player player = game.Players.Find(p => p.Id == playerId);

                if (player == null)
                    throw new GameException("Player not found");

                if (player.Status != StatusEnum.SETUP)
                    throw new GameException("Player not in setup mode: " + player.Status);

                player.Status = StatusEnum.PLAYING;

                if (! game.Players.Exists(p => p.Status != StatusEnum.PLAYING))
                {
                    game.Status = StatusEnum.PLAYING;
                    game.PlayerNameTurn = shitheadService.ChooseFirstTurn(game.Players);
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

                if (game == null)
                    throw new GameException("Game not found");

                if (game.Status != StatusEnum.PLAYING)
                    throw new GameException("Game not in playing mode: " + game.Status);

                Player player = game.Players.Find(p => p.Id == playerId);

                if (player == null)
                    throw new GameException("Player not found");

                if (player.Status != StatusEnum.PLAYING)
                    throw new GameException("Player not in playing mode: " + player.Status);

                if (player.Name != game.PlayerNameTurn)
                    throw new GameException("Player not in turn");

                player.InHandCards.AddRange(game.TableCards);
                game.TableCards.Clear();

                game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);

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

                if (game == null)
                    throw new GameException("Game not found");

                if (game.Status != StatusEnum.PLAYING)
                    throw new GameException("Game not in playing mode: " + game.Status);

                Player player = game.Players.Find(p => p.Id == playerId);

                if (player == null)
                    throw new GameException("Player not found");

                if (player.Status != StatusEnum.PLAYING)
                    throw new GameException("Player not in playing mode: " + player.Status);

                if (player.Name != game.PlayerNameTurn)
                    throw new GameException("Player not in turn");

                DiscardResult result;

                if (cards == "down")
                {
                    if (player.InHandCards.Count != 0 || player.OpenCards.Count != 0)
                        throw new GameException("Player cannot play down card with open or hand cards");

                    var cardToBePlayed = player.DownCards[0];
                    player.DownCards.RemoveAt(0);

                    result = shitheadService.EvaluateCardsOnTable(new List<string> { cardToBePlayed }, game.TableCards);

                    if (result == DiscardResult.Refuse)
                    {
                        player.InHandCards.Add(cardToBePlayed);
                        player.InHandCards.AddRange(game.TableCards);
                        game.TableCards.Clear();
                        game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);
                    }
                    else if (result == DiscardResult.Ok)
                    {
                        game.TableCards.Add(cardToBePlayed);
                        game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);
                    } 
                    else if (result == DiscardResult.OkBurned)
                    {
                        game.LastBurnedCard = cardToBePlayed;
                        game.BurnedCardsCount = game.TableCards.Count + 1;
                        game.TableCards.Clear();

                        if (player.DownCards.Count == 0)
                        {
                            player.Status = StatusEnum.OUT;
                            game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);
                        }
                    } 
                }
                else
                {
                    List<string> cardsToBePlayed = cards.Split(",").ToList();
                    if (cardsToBePlayed.Count > 1 && !cardsToBePlayed.All(c => shitheadService.SameNumber(c, cardsToBePlayed[0])))
                        throw new GameException("Player cannot discard multiple cards with different numbers");

                    if (player.InHandCards.Count > 0 && ! cardsToBePlayed.All(c => player.InHandCards.Contains(c)))
                        throw new GameException("Player hand does not contain the cards to discard");

                    if (player.InHandCards.Count == 0 && !cardsToBePlayed.All(c => player.OpenCards.Contains(c)))
                        throw new GameException("Player open cards does not contain the cards to discard");

                    result = shitheadService.EvaluateCardsOnTable(cardsToBePlayed, game.TableCards);

                    if (result == DiscardResult.Ok)
                    {
                        game.TableCards.AddRange(cardsToBePlayed);

                        if (player.InHandCards.Count > 0)
                        {
                            player.InHandCards.RemoveAll(c => cardsToBePlayed.Contains(c));
                        }
                        else
                        {
                            player.OpenCards.RemoveAll(c => cardsToBePlayed.Contains(c));
                        }

                        if (player.DownCards.Count == 0 && player.InHandCards.Count == 0 && player.OpenCards.Count == 0)
                        {
                            player.Status = StatusEnum.OUT;
                            game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);
                        }
                        else
                        { 
                            string cardNumber = shitheadService.GetCardNumber(cardsToBePlayed[0]);

                            int stepToNextTurn = cardNumber == "8" ? cardsToBePlayed.Count + 1 : 1;

                            game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, stepToNextTurn);
                        }
                    }
                    else if (result == DiscardResult.OkBurned)
                    {
                        game.LastBurnedCard = cardsToBePlayed[0];
                        game.BurnedCardsCount = game.TableCards.Count + cardsToBePlayed.Count;
                        game.TableCards.Clear();

                        if (player.InHandCards.Count > 0)
                        {
                            player.InHandCards.RemoveAll(c => cardsToBePlayed.Contains(c));
                        }
                        else
                        {
                            player.OpenCards.RemoveAll(c => cardsToBePlayed.Contains(c));
                        }

                        if (player.DownCards.Count == 0 && player.InHandCards.Count == 0 && player.OpenCards.Count == 0)
                        {
                            player.Status = StatusEnum.OUT;
                            game.PlayerNameTurn = shitheadService.NextPlayerFrom(game.Players, playerId, 1);
                        }

                    }

                    if (game.CardsInDeck.Count > 0 && player.InHandCards.Count < 3)
                    {
                        int numToBuy = 3 - player.InHandCards.Count;
                        player.InHandCards.AddRange(game.CardsInDeck.Take(numToBuy));
                        game.CardsInDeck.RemoveRange(0, numToBuy);
                    }
                }

                SaveGame(game);

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

    }
}
