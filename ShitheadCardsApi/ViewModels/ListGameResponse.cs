using ShitheadCardsApi.Models;
using System;
using System.Linq;

namespace ShitheadCardsApi.ViewModels
{
    public class ListGameResponse
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public int PlayersCount { get; set; }
        public DateTime DateCreated { get; set; }

        public ListGameResponse(Game game)
        {
            Name = game.Name;
            CreatedBy = game.Players[0].Name;
            PlayersCount = game.Players.Count;
            DateCreated = game.DateCreated;
        }
    }
}
