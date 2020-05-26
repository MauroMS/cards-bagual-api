using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ShitheadCardsApi.Moq
{
    public class GameServiceTests
    {
        //system under test
        private readonly GameService _sut;
        private readonly GameService _sutMock;
        private List<Player> players;
        private Mock<ShitheadService> shitheadService;
        private Mock<IShitheadService> shitheadServiceMoq;
        private ShitheadDBContext dbContext;

        public GameServiceTests()
        {
            shitheadService = new Mock<ShitheadService>();
            shitheadServiceMoq = new Mock<IShitheadService>();
            CreateNewDb();
            _sut = new GameService(dbContext, shitheadService.Object);
            _sutMock = new GameService(dbContext, shitheadServiceMoq.Object);
        }

        private List<Player> CreatePlayersListMock(int playersNeeded)
        {
            players = new List<Player>();

            if (playersNeeded > 0)
                players.Add(new Player()
                {
                    Id = "2423423fsdf",
                    Name = "Juanito0",
                    InHandCards = new List<string>() { "0S", "7D", "0H" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 1)
                players.Add(new Player()
                {
                    Id = "4534g34g",
                    Name = "Igor1",
                    InHandCards = new List<string>() { "AH", "AD", "AS" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 2)
                players.Add(new Player()
                {
                    Id = "asdfsdf34",
                    Name = "Bob2",
                    InHandCards = new List<string>() { "KH", "KD", "KS" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 3)
                players.Add(new Player()
                {
                    Id = "43t5y4y ",
                    Name = "Joao3",
                    InHandCards = new List<string>() { "3H", "6D", "8S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 4)
                players.Add(new Player()
                {
                    Id = "34645g4hy65u",
                    Name = "Marquito4",
                    InHandCards = new List<string>() { "JH", "4D", "9S" },
                    Status = StatusEnum.PLAYING
                });

            return players;
        }

        [Fact]
        public void CreateGame_CreatesANewGameWithSetupStatusAndNewDeck()
        {
            // Arrange
            var gameName = "Game name";

            // Act
            var game = _sutMock.CreateGame(gameName);

            // Assert
            Assert.Equal(gameName, game.Name);
            Assert.Equal(StatusEnum.SETUP, game.Status);
            shitheadServiceMoq.Verify(mock => mock.CreateDeck(), Times.Once());
        }

        [Fact]
        public void CreateOrJoinGame_ShouldReturnCorrectAmountOfCardsInDeck()
        {
            // Arrange
            var gameName = "Game name";
            var game = _sut.CreateOrJoinGame(gameName, "Locomia");
            game = _sut.CreateOrJoinGame(gameName, "Locomia1");
            game = _sut.CreateOrJoinGame(gameName, "Locomia2");

            // Act
            game = _sut.GetGame(gameName);

            // Assert
            Assert.Equal(game.CardsInDeck.Count, (52 - (9 * game.Players.Count)));
        }

        [Fact]
        public void DiscardPlayerCards_ShouldReturnCorrectBurnedCardsCount()
        {
            // Arrange
            var gameName = "Game name";
            var game = new Game()
            {
                BurnedCardsCount = 0,
                CardsInDeck = new List<string>() { "2A", "3H", "7S", "8S" },
                DateCreated = DateTime.Now,
                Name = gameName,
                LastBurnedCard = "6S",
                Status = StatusEnum.PLAYING,
                TableCards = new List<string>() { "2A", "3A", "4A", "5H", "7H", "0A", "5A", "8H", "9H", "3H", "7H", "3S", "4S", "7S", "9S" }, // 15 Cards on the table
                Players = CreatePlayersListMock(2)
            };

            game.PlayerNameTurn = game.Players[0].Name;

            var gameDbModel = new GameDbModel()
            {
                Name = game.Name,
                GameJson = _sut.Serialize(game)
            };

            dbContext.Add(gameDbModel);
            dbContext.SaveChanges();

            // Act
            game = _sut.DiscardPlayerCards(game.Name, game.Players[0].Id, "0S");

            // Assert
            Assert.Empty(game.TableCards);
            Assert.Equal(16, game.BurnedCardsCount);
        }

        [Fact]
        public void DiscardPlayerCards_ShouldReturnException_NotPlayersTurn()
        {
            // Arrange
            var gameName = "Game name";
            var game = new Game()
            {
                BurnedCardsCount = 0,
                CardsInDeck = new List<string>() { "2A", "3H", "7S", "8S" },
                DateCreated = DateTime.Now,
                Name = gameName,
                LastBurnedCard = "6S",
                Status = StatusEnum.PLAYING,
                TableCards = new List<string>() { "2A", "3A", "4A", "5H", "7H", "0A", "5A", "8H", "9H", "3H", "7H", "3S", "4S", "7S", "9S" }, // 15 Cards on the table
                Players = CreatePlayersListMock(2)
            };

            game.PlayerNameTurn = game.Players[0].Name;

            var gameDbModel = new GameDbModel()
            {
                Name = game.Name,
                GameJson = _sut.Serialize(game)
            };

            dbContext.Add(gameDbModel);
            dbContext.SaveChanges();

            // Act
            var ex = Assert.Throws<GameException>(() => _sut.DiscardPlayerCards(game.Name, game.Players[1].Id, "0S"));

            // Assert
            Assert.Equal("Player not in turn", ex.Message);
        }

        private void CreateNewDb()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<ShitheadDBContext>();
            builder.UseInMemoryDatabase(databaseName: "ShitheadGamesTest")
               .UseInternalServiceProvider(serviceProvider);

            dbContext = new ShitheadDBContext(builder.Options);
        }
    }
}
