using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.ViewModels
{
    public class CreateOrJoinGameResponse
    {
        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public string PlayerId { get; set; }

        public CreateOrJoinGameResponse(Game game, string playerName)
        {
            this.Name = game.Name;
            this.Status = game.Status;
            this.PlayerId = game.Players.Find(p => p.Name == playerName).Id;
        }
    }
}
