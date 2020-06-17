using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.Controllers
{
    internal class AddBotPlayerResponse
    {
        private Game game;

        public AddBotPlayerResponse(Game game)
        {
            this.game = game;
        }
    }
}