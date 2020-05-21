using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace ShitheadCardsApi.Moq
{
    public class ShitheadServiceTests
    {
        //system under test
        private readonly ShitheadService _sut;

        public ShitheadServiceTests()
        {
            _sut = new ShitheadService();
        }

        [Fact]
        public void ChooseFirstTurn_ShouldReturnPlayer_WithLowestCard()
        {
            // Arrange
            List<Player> players = new List<Player>();
            players.AddRange(new List<Player>() {
                new Player()
                {
                    Id = "2423423fsdf",
                    Name = "Juanito",
                    InHandCards = new List<string>() { "2H", "7D", "5S" }
                },
                new Player()
                {
                    Id = "4534g34g",
                    Name = "Igor",
                    InHandCards = new List<string>() { "AH", "9D", "7S" }
                },
                new Player()
                {
                    Id = "asdfsdf34",
                    Name = "Bob",
                    InHandCards = new List<string>() { "KH", "JD", "4S" }
                },
                new Player()
                {
                    Id = "43t5y4y ",
                    Name = "Joao",
                    InHandCards = new List<string>() { "3H", "6D", "8S" }
                },
                new Player()
                {
                    Id = "34645g4hy65u",
                    Name = "Marquito",
                    InHandCards = new List<string>() { "JH", "4D", "9S" }
                }
                });

            // Act
            var firstPlayer = _sut.ChooseFirstTurn(players);

            // Assert
            Assert.Equal("Bob", firstPlayer);

        }
    }
}
