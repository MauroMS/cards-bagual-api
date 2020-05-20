using ShitheadCardsApi.Models;
using System.Linq;

namespace ShitheadCardsApi.ViewModels
{
    public class CreateOrJoinGameResponse
    {
        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public string PlayerId { get; set; }

        public CreateOrJoinGameResponse(Game game, string playerName)
        {
            Name = game.Name;
            Status = game.Status;
            PlayerId = game.Players.FirstOrDefault(p => p.Name == playerName)?.Id;
        }
    }
}
