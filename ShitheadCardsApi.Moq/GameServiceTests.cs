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
            var game = CreateGameMock(gameName, StatusEnum.PLAYING, 2, new List<string>() { "7S", "9S" });
            var gameDbModel = CreateGameDbModelMock(game);
           
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

        [Fact]
        public void CreateOrJoinGame_ShouldReturnCorrectAmountOfCardsInDeck()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 1);
            var gameDbModel = CreateGameDbModelMock(game);

            shitheadDbContext.Setup(x => x.Find<GameDbModel>(gameName))
                .Returns(gameDbModel);

            // Act
            _sut.CreateOrJoinGame(gameName, "ErvaMate");
            _sut.CreateOrJoinGame(gameName, "CuiaDePlastica");

            // Assert
            Assert.Equal((game.CardsInDeck.Count - 18), _sut.GetGame(gameName).CardsInDeck.Count);
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP1_2Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 2);

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[0].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.DoesNotContain(game.Players[0].Id, sortedPlayers.Select(p => p.Id));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP2_2Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 2);

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[1].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.DoesNotContain(game.Players[1].Id, sortedPlayers.Select(p => p.Id));
        }


        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP1_3Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 3);
            var expectedPlayerOrder = new List<string>() { game.Players[1].Name, game.Players[2].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[0].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP2_3Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 3);
            var expectedPlayerOrder = new List<string>() { game.Players[2].Name, game.Players[0].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[1].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP3_3Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 3);
            var expectedPlayerOrder = new List<string>() { game.Players[0].Name, game.Players[1].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[2].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }


        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP1_4Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 4);
            var expectedPlayerOrder = new List<string>() { game.Players[2].Name, game.Players[3].Name, game.Players[1].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[0].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP2_4Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 4);
            var expectedPlayerOrder = new List<string>() { game.Players[3].Name, game.Players[0].Name, game.Players[2].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[1].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP3_4Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 4);
            var expectedPlayerOrder = new List<string>() { game.Players[0].Name, game.Players[1].Name, game.Players[3].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[2].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP4_4Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 4);
            var expectedPlayerOrder = new List<string>() { game.Players[1].Name, game.Players[2].Name, game.Players[0].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[3].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }


        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP1_5Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 5);
            var expectedPlayerOrder = new List<string>() { game.Players[3].Name, game.Players[4].Name, game.Players[2].Name, game.Players[1].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[0].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP2_5Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 5);
            var expectedPlayerOrder = new List<string>() { game.Players[4].Name, game.Players[0].Name, game.Players[3].Name, game.Players[2].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[1].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP3_5Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 5);
            var expectedPlayerOrder = new List<string>() { game.Players[0].Name, game.Players[1].Name, game.Players[4].Name, game.Players[3].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[2].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP4_5Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 5);
            var expectedPlayerOrder = new List<string>() { game.Players[1].Name, game.Players[2].Name, game.Players[0].Name, game.Players[4].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[3].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }

        [Fact]
        public void GetOtherPlayers_ShouldReturnCorrectOrderOfPlayers_CurrentUserP5_5Players()
        {
            // Arrange
            var gameName = "Game name";
            var game = CreateGameMock(gameName, StatusEnum.SETUP, 5);
            var expectedPlayerOrder = new List<string>() { game.Players[2].Name, game.Players[3].Name, game.Players[1].Name, game.Players[0].Name };

            // Act
            var sortedPlayers = GameHelper.GetOtherPlayers(game.Players, game.Players[4].Id);

            // Assert
            Assert.Equal(game.Players.Count - 1, sortedPlayers.Count);
            Assert.Equal(expectedPlayerOrder, sortedPlayers.Select(p => p.Name));
        }



        private Game CreateGameMock(string name, StatusEnum status, int playersNeeded, List<string> tableCards = null)
        {
            var game = new Game()
            {
                BurnedCardsCount = 0,
                CardsInDeck = new List<string>()
                { "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S",
                  "2A", "3H", "7S", "8S"
                },
                DateCreated = DateTime.Now,
                Name = name,
                LastBurnedCard = "",
                Status = status,
                TableCards = tableCards ?? new List<string>(),
                Players = CreatePlayersListMock(playersNeeded)
            };

            game.PlayerNameTurn = game.Players[0].Name;

            return game;
        }

        private GameDbModel CreateGameDbModelMock(Game game)
        {
            var gameDbModel = new GameDbModel()
            {
                Name = game.Name,
                GameJson = _sut.Serialize(game)
            };

            return gameDbModel;
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
