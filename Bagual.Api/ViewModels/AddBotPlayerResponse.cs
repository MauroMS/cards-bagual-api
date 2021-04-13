using Bagual.Services.Shithead.Models;

namespace Bagual.Api.ViewModels
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