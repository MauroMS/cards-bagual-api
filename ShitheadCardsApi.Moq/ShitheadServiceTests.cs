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


        // No Cards on the table - Return OK
        // No Cards on the table + 4 of the same number - Return OkBurned
        // Lower card than the one on the table - Return Refuse (Other than 2/3/10)
        // Higher card than the one on the table - Return Ok
        // Single same card as the one on the table (Not 4 of the same) - Return Ok
        // Two card the same as the one on the table (3 in total) - Return 
        // Three card the same as the one on the table (4 in total) - Return OkBurned
        // Play 2 - Return Ok
        // Play 3 - Return Ok
        // Play Higher card when there is a 7 on the table - Return Refuse
        // Play Lower card when there is a 7 on the table - Return Ok
        // Play 10

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_NoCardsOnTheTable()
        {
            // Arrange

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "2A" }, new List<string>());

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_NoCardsOnTheTable4OfTheSame() {
            // Arrange

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "2A", "2H", "2C", "2S" }, new List<string>());

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);

        }


        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_CardNotValidToPlay() {
            // Arrange


            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "7H" }, new List<string>() { "9A" });

            // Assert
            Assert.True(discardResult == DiscardResult.Refuse);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_CardValidToPlay()
        {
            // Arrange


            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "9H" }, new List<string>() { "9A" });

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_BurnerCardValidToPlayLowerThan10()
        {
            // Arrange


            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "0H" }, new List<string>() { "9A" });

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_BurnerCardValidToPlayHigherThan10()
        {
            // Arrange


            // Act
            var discardResult = _sut.EvaluateCardsOnTable(new List<string>() { "0H" }, new List<string>() { "KA" });

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_CardNotValidToPlay()
        {
            // Arrange


            // Act
            var test = _sut.EvaluateCardsOnTable(new List<string>() { "9H" }, new List<string>() { "5A" });

            // Assert

        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_CardNotValidToPlay()
        {
            // Arrange


            // Act
            var test = _sut.EvaluateCardsOnTable(new List<string>() { "9H" }, new List<string>() { "5A" });

            // Assert

        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_CardNotValidToPlay()
        {
            // Arrange


            // Act
            var test = _sut.EvaluateCardsOnTable(new List<string>() { "9H" }, new List<string>() { "5A" });

            // Assert

        }
    }
}
