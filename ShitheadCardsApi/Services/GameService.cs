using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShitheadCardsApi
{
    public class GameService : IGameService
    {
        private ShitheadDBContext _ctx;
        private IShitheadService shitheadService;
        public GameService(ShitheadDBContext ctx, IShitheadService shitheadService)
        {
            _ctx = ctx;
            this.shitheadService = shitheadService;
        }

        public Game CreateOrJoinGame(string gameName, string playerName)
        {
            List<string> cards = shitheadService.CreateDeck();

            var game = new Game
            {
                Name = gameName,
                BurnedCardsCount = 5,
                CardsInDeck = cards,
                LastBurnedCard = "5G",
                Players = {
                        new Player() {
                        Id = 1,
                        Name = "Dilmae",
                        DownCards = {},
                        InHandCards = {},
                        OpenCards = {},
                        Status = (int) StatusEnum.PLAYING
                        },
                        new Player() {
                        Id = 2,
                        Name = "Bozonaro",
                        DownCards = {},
                        InHandCards = {},
                        OpenCards = {},
                        Status = (int) StatusEnum.PLAYING
                        }
                },
                PlayerTurn = 1,
                TableCards = { }
            };

            SaveGame(game);

            return FindGame(gameName);
        }

        private void SaveGame(Game game)
        {
            var dbModel = new GameDbModel()
            {
                Name = game.Name,
                GameJson = Serialize(game)
            };

            _ctx.ShitheadGames.Add(dbModel);
            _ctx.SaveChanges();
        }

        private Game FindGame(string name)
        {
            return Deserialize(_ctx.Find<GameDbModel>(name));
        }

        public Game GetGame(string name)
        {
            return Deserialize(_ctx.Find<GameDbModel>(name));
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
