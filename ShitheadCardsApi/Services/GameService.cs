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
            object gameLock = locker.GetOrAdd(gameName, new Object());

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
                throw new Exception("Game not in setup mode: " + game.Status);
            }

            var player = game.Players.Find(player1 => player1.Name.Equals(playerName));

            if (player == null)
            {
                if (game.Players.Count == 5)
                {
                    throw new Exception("Max number of players reached: 5");
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
            } else
            {
                gameDbModel.GameJson = Serialize(game);
            }
            
            _ctx.SaveChanges();
        }

        public Game GetGame(string name)
        {
            GameDbModel gameDbModel = _ctx.Find<GameDbModel>(name);
            if (gameDbModel == null)
                return null;
            return Deserialize(gameDbModel);
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

        public Game GetGame(string gameName, string playerId)
        {
            throw new NotImplementedException();
        }

        public Game SwitchPlayerCards(string gameName, string playerId, string openCard, string handCard)
        {
            throw new NotImplementedException();
        }

        public Game SetPlayerToStart(string gameName, object playerId)
        {
            throw new NotImplementedException();
        }

        public Game DiscardPlayerCards(string gameName, string playerId, string cards)
        {
            throw new NotImplementedException();
        }

        public Game MoveTableCardsToPlayer(string gameName, string playerId)
        {
            throw new NotImplementedException();
        }
    }
}
