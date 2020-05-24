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

    }
}
