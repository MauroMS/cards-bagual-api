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
        public GameService(ShitheadDBContext ctx)
        {
            _ctx = ctx;
        }

        List<string> allCards = new List<string>(){ "2H", "2D", "2C", "2S", "3H", "3D", "3C", "3S", "4H", "4D", "4C", "4S", "5H", "5D", "5C", "5S", "6H", "6D", "6C", "6S", "7H", "7D", "7C", "7S", "8H", "8D", "8C", "8S", "9H", "9D", "9C", "9S", "0H", "0D", "0C", "0S", "JH", "JD", "JC", "JS", "QH", "QD", "QC", "QS", "KH", "KD", "KC", "KS", "AH", "AD", "AC", "AS" };

        public void CreateGame()
        {
            var game = new Game
            {
                Name = "Test",
                BurnedCardsCount = 5,
                CardsInDeck = { },
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

            var dbModel = new GameDbModel()
            {
                Name = "Test",
                GameJson = Serialize(game)
            };

            _ctx.ShitheadGames.Add(dbModel);
            _ctx.SaveChanges();
        }

        public Game GetGame(string name)
        {
            return Deserialize(_ctx.ShitheadGames.FirstOrDefault(game => game.Name.Equals(name)));
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
