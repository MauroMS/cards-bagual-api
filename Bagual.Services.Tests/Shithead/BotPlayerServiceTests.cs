using Bagual.Services.Shithead.Interfaces;
using Bagual.Services.Shithead.Models;
using Bagual.Services.Shithead.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bagual.Services.Tests.Shithead
{
    public class BotPlayerServiceTests
    {
        //system under test
        private readonly BotPlayerService _sut;
        private Mock<IShitheadService> shitheadService;

        public BotPlayerServiceTests()
        {
            shitheadService = new Mock<IShitheadService>();
            _sut = new BotPlayerService(shitheadService.Object);
        }

        [Fact]
        public void InitializePlayer_ShouldSetTheBestOpenCards()
        {
            // Arrange
            var players = new List<Player>() {
                new Player()
                {
                    Id = "2423423fsdf",
                    Name = "Bot",
                    InHandCards = new List<string>() { "KH", "0D", "5S" },
                    OpenCards = new List<string>() { "3H", "AD", "8S" },
                    Status = StatusEnum.SETUP,
                    Bot = true
                }
            };

            var game = new Game()
            {
                BurnedCardsCount = 0,
                DateCreated = DateTime.Now,
                TableCards = new List<string>(),
                Players = players
            };

            game.PlayerNameTurn = game.Players[0].Name;

            // Act
            _sut.InitializePlayer(game, game.PlayerNameTurn);

            // Assert
            Assert.Equal(new List<string>() { "AD", "3H", "0D" }.ToHashSet(), players[0].OpenCards.ToHashSet());
            Assert.Equal(new List<string>() { "KH", "5S", "8S" }.ToHashSet(), players[0].InHandCards.ToHashSet());
            Assert.Equal(StatusEnum.PLAYING, players[0].Status);
        }

    }
}