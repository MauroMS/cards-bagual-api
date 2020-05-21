using System;
using System.Collections.Generic;

namespace ShitheadCardsApi.Models
{
    public class Game
    {
        public Game()
        {
            Players = new List<Player>();
            TableCards = new List<string>();
            CardsInDeck = new List<string>();
            Status = StatusEnum.SETUP;
        }

        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public List<Player> Players { get; set; }
        public List<string> TableCards { get; set; }
        public string LastBurnedCard { get; set; }
        public int BurnedCardsCount { get; set; }
        public List<string> CardsInDeck { get; set; }
        public string PlayerIdTurn { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
