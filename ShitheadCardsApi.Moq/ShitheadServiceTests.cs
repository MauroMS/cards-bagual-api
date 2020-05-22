using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ShitheadCardsApi.Moq
{
    public class ShitheadServiceTests
    {
        //system under test
        private readonly ShitheadService _sut;
        private List<Player> players;

        public ShitheadServiceTests()
        {
            _sut = new ShitheadService();
        }

        private List<Player> CreatePlayersListMock(int playersNeeded)
        {
            players = new List<Player>();

            if (playersNeeded > 0)
                players.Add(new Player()
                {
                    Id = "2423423fsdf",
                    Name = "Juanito",
                    InHandCards = new List<string>() { "2H", "7D", "5S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 1)
                players.Add(new Player()
                {
                    Id = "4534g34g",
                    Name = "Igor",
                    InHandCards = new List<string>() { "AH", "9D", "7S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 2)
                players.Add(new Player()
                {
                    Id = "asdfsdf34",
                    Name = "Bob",
                    InHandCards = new List<string>() { "KH", "JD", "4S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 3)
                players.Add(new Player()
                {
                    Id = "43t5y4y ",
                    Name = "Joao",
                    InHandCards = new List<string>() { "3H", "6D", "8S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 4)
                players.Add(new Player()
                {
                    Id = "34645g4hy65u",
                    Name = "Marquito",
                    InHandCards = new List<string>() { "JH", "4D", "9S" },
                    Status = StatusEnum.PLAYING
                });

            return players;
        }

        [Fact]
        public void ChooseFirstTurn_ShouldReturnPlayer_WithLowestCard()
        {
            // Arrange
            players = CreatePlayersListMock(5);

            // Act
            var firstPlayer = _sut.ChooseFirstTurn(players);

            // Assert
            Assert.Equal("Bob", firstPlayer);

        }

        [Fact]
        public void CreateDeck_ShouldReturnSortedDeck_With52Cards()
        {
            // Arrange

            // Act
            var deck = _sut.CreateDeck();

            // Assert
            Assert.True(deck.Count == 52);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnNextPlayer_NoSkipPlayer()
        {
            // Arrange
            players = CreatePlayersListMock(3);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 1);

            // Assert
            Assert.Equal(nextPlayer, players[1].Name);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnNextPlayer_Skip1Player()
        {
            // Arrange
            players = CreatePlayersListMock(3);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 2);

            // Assert
            Assert.Equal(nextPlayer, players[2].Name);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnNextPlayer_Skip2Players()
        {
            // Arrange
            players = CreatePlayersListMock(5);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 3);

            // Assert
            Assert.Equal(nextPlayer, players[3].Name);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnSamePlayer_Skip3Players()
        {
            // Arrange
            players = CreatePlayersListMock(3);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 4);

            // Assert
            Assert.Equal(nextPlayer, players[0].Name);
        }
    }
}
