using Bagual.Services.Shithead.Models;
using Bagual.Services.Shithead.Services;
using System.Collections.Generic;
using Xunit;

namespace Bagual.Services.Tests
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
                    Name = "Juanito0",
                    InHandCards = new List<string>() { "2H", "7D", "5S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 1)
                players.Add(new Player()
                {
                    Id = "4534g34g",
                    Name = "Igor1",
                    InHandCards = new List<string>() { "AH", "9D", "7S" },
                    Status = StatusEnum.PLAYING
                });

            if (playersNeeded > 2)
                players.Add(new Player()
                {
                    Id = "asdfsdf34",
                    Name = "Bob2",
                    InHandCards = new List<string>() { "KH", "JD", "4S" },
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
        public void ChooseFirstTurn_ShouldReturnPlayer_WithLowestCard()
        {
            // Arrange
            players = CreatePlayersListMock(5);

            // Act
            var firstPlayer = _sut.ChooseFirstTurn(players);

            // Assert
            Assert.Equal("Bob2", firstPlayer);

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
            Assert.Equal(players[1].Name, nextPlayer);
        }

        [Fact]
        public void NextPlayerFrom_ShouldConsiderPlayerOutWithTwoPlayers()
        {
            // Arrange
            players = CreatePlayersListMock(2);
            players[0].Status = StatusEnum.OUT;

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 1);

            // Assert
            Assert.Equal(players[1].Name, nextPlayer);
        }

        [Fact]
        public void NextPlayerFrom_ShouldConsiderPlayerOutWithThrePlayers()
        {
            // Arrange
            players = CreatePlayersListMock(3);
            players[1].Status = StatusEnum.OUT;

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 1);

            // Assert
            Assert.Equal(players[2].Name, nextPlayer);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnNextPlayer_Skip1Player()
        {
            // Arrange
            players = CreatePlayersListMock(3);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 2);

            // Assert
            Assert.Equal(players[2].Name, nextPlayer);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnNextPlayer_Skip2PlayersSkippingOut()
        {
            // Arrange
            players = CreatePlayersListMock(5);
            players[2].Status = StatusEnum.OUT;

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 3);

            // Assert
            Assert.Equal(players[4].Name, nextPlayer);
        }

        [Fact]
        public void NextPlayerFrom_ShouldReturnSamePlayer_Skip3Players()
        {
            // Arrange
            players = CreatePlayersListMock(3);

            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[0].Id, 4);

            // Assert
            Assert.Equal(players[0].Name, nextPlayer);
        }


        [Fact]
        public void NextPlayerFrom_ShouldConsiderPlayersOutWhenSkippingPlayers()
        {
            // Arrange
            players = CreatePlayersListMock(5);
            players[4].Status = StatusEnum.OUT;


            // Act
            var nextPlayer = _sut.NextPlayerFrom(players, players[3].Id, 2);

            // Assert
            Assert.Equal(players[1].Name, nextPlayer);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_NoCardsOnTheTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "2A" };
            List<string> tableCards = new List<string>() { };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_NoCardsOnTheTable4OfTheSame() {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "2A", "2H", "2C", "2S" };
            List<string> tableCards = new List<string>() { };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_LowerCardThanTableTop() {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "7H" };
            List<string> tableCards = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Refuse);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_HigherCardThanTableTop()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H" };
            List<string> tableCards = new List<string>() { "4A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_SameCardValidToPlay1InHand1InTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H" };
            List<string> tableCards = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_SameCardValidToPlay2InHand1InTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H", "9D" };
            List<string> tableCards = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_SameCardValidToPlay3InHand1InTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H", "9D", "9S" };
            List<string> tableCards = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefused_SameCardInvalidToPlay4OfTheSame()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H", "9D", "9S", "9A" };
            List<string> tableCards = new List<string>() { "0A", "KS" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Refuse);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_SameCardValidToPlay4OfTheSame()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H", "9D", "9S", "9A" };
            List<string> tableCards = new List<string>() { "2A", "4S" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_SameCardValidToPlay1InHand2InTable()
        {
            // Arrange
            List<string> tableCards = new List<string>() { "9H", "9D" };
            List<string> cardsToBePlayed = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_SameCardValidToPlay1InHand3InTable()
        {
            // Arrange
            List<string> tableCards = new List<string>() { "9H", "9D", "9S" };
            List<string> cardsToBePlayed = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_SameCardValidToPlay2InHand2InTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9H", "9D" };
            List<string> tableCards = new List<string>() { "9A", "9C" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_ValidResetCard()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "2A" };
            List<string> tableCards = new List<string>() { "9A", "9C" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_ValidMirrorCard()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "3A" };
            List<string> tableCards = new List<string>() { "9A", "9C" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }


        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOk_ValidMirrorCardOnTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "5A" };
            List<string> tableCards = new List<string>() { "7A", "3A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_BurnerCardValidToPlayWhenLowerThan10()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "0H" };
            List<string> tableCards = new List<string>() { "9A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultOkBurned_BurnerCardValidToPlayWhenHigherThan10()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "0H" };
            List<string> tableCards = new List<string>() { "KA" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.OkBurned);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_HigherCardNotValidToPlayWhen7OnTheTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "9A" };
            List<string> tableCards = new List<string>() { "7H" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Refuse);
        }

        [Fact]
        public void EvaluateCardsOnTable_ShouldReturnResultRefuse_LowerCardValidToPlayWhen7OnTheTable()
        {
            // Arrange
            List<string> cardsToBePlayed = new List<string>() { "5H" };
            List<string> tableCards = new List<string>() { "7A" };

            // Act
            var discardResult = _sut.EvaluateCardsOnTable(cardsToBePlayed, tableCards);

            // Assert
            Assert.True(discardResult == DiscardResult.Ok);
        }
    }
}
