using Moq;
using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ShitheadCardsApi.Moq
{
    public class GameServiceTests
    {
        //system under test
        private readonly GameService _sut;
        private List<Player> players;
        private Mock<IShitheadService> shitheadService;
        private Mock<ShitheadDBContext> shitheadDbContext;

        public GameServiceTests()
        {
            shitheadDbContext = new Mock<ShitheadDBContext>();
            shitheadService = new Mock<IShitheadService>();
            _sut = new GameService(shitheadDbContext.Object, shitheadService.Object);
        }

        [Fact]
        public void CreateGame_CreatesANewGameWithSetupStatusAndNewDeck()
        {
            // Arrange
            var gameName = "Game name";

            // Act
            var game = _sut.CreateGame(gameName);

            // Assert
            Assert.Equal(gameName, game.Name);
            Assert.Equal(StatusEnum.SETUP, game.Status);
            shitheadService.Verify(mock => mock.CreateDeck(), Times.Once());
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
                TableCards = new List<string>() {  "7S", "9S" }, 
                Players = CreatePlayersListMock(2)
            };

            game.PlayerNameTurn = game.Players[0].Name;

            var gameDbModel = new GameDbModel()
            {
                Name = gameName,
                GameJson = _sut.Serialize(game)
            };

            shitheadDbContext.Setup(x => x.Find<GameDbModel>(gameName))
                .Returns(gameDbModel);

            shitheadService.Setup(x => x.EvaluateCardsOnTable(It.IsAny<List<string>>(), game.TableCards))
                .Returns(DiscardResult.OkBurned);

            // Act
            game = _sut.DiscardPlayerCards(game.Name, game.Players[0].Id, "0S");

            // Assert
            Assert.Empty(game.TableCards);
            Assert.Equal(3, game.BurnedCardsCount);
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
    }
}
