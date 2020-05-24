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
            var game = _sut.CreateGame(gameName);
            game.Players.AddRange(CreatePlayersListMock(2));
            game.TableCards.AddRange(new List<string>() { "2A", "3A", "4A", "5H", "7H", "0A", "5A", "8H", "9H", "3H", "7H", "3S", "4S", "7S", "9S" }); // 15 Cards on the table

            //https://www.jankowskimichal.pl/en/2016/01/mocking-dbcontext-and-dbset-with-moq/
            //https://stackoverflow.com/questions/25960192/mocking-ef-dbcontext-with-moq

            shitheadDbContext.Object.Add(game);
            shitheadDbContext.Object.SaveChanges();
            var a = shitheadDbContext.Object.Find<GameDbModel>(gameName);

            // Act
            _sut.DiscardPlayerCards(game.Name, game.Players[0].Id, "0S");

            // Assert
            Assert.Equal(game.TableCards.Count, 0);
            Assert.Equal(game.BurnedCardsCount, 16);
        }

        [Fact]
        public void DiscardPlayerCards_ShouldReturnCorrectBurnedCardsCount_2ndTime()
        {
            // Arrange
            var gameName = "Game name";
            var game = _sut.CreateGame(gameName);
            game.Players.AddRange(CreatePlayersListMock(1));

            players.Add(new Player()
            {
                Id = "4534g34g",
                Name = "Igor1",
                InHandCards = new List<string>() { "AH", "AD", "AS" },
                Status = StatusEnum.PLAYING
            });

            players.Add(new Player()
            {
                Id = "asdfsdf34",
                Name = "Bob2",
                InHandCards = new List<string>() { "KH", "KD", "KS", "8S", "8A" },
                Status = StatusEnum.PLAYING
            });

            game.TableCards.AddRange(new List<string>() { "2A", "3A", "4A", "5H", "7H", "0A", "5A", "8H", "9H", "3H", "7H", "3S", "4S", "7S", "9S" }); // 15 Cards on the table

            // Act
            _sut.DiscardPlayerCards(game.Name, game.Players[0].Id, "0S");
            _sut.DiscardPlayerCards(game.Name, game.Players[1].Id, "AH,AD,AS");
            _sut.DiscardPlayerCards(game.Name, game.Players[2].Id, "KH,KD,KS");
            _sut.DiscardPlayerCards(game.Name, game.Players[0].Id, "0H");

            // Assert
            Assert.Equal(game.TableCards.Count, 0);
            Assert.Equal(game.BurnedCardsCount, 23);
        }

    }
}
