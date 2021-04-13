using Bagual.Services.Shithead.Models;

namespace Bagual.Services.Shithead.Interfaces
{
    public interface IBotPlayerService
    {
        string GeneratePlayerName(Game game);
        void InitializePlayer(Game game, string playerName);
        string PlayBotPlayerTurn(Game game);
    }
}