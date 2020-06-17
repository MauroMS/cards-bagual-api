using ShitheadCardsApi.Models;

namespace ShitheadCardsApi
{
    public interface IBotPlayerService
    {
        string GeneratePlayerName(Game game);
        void InitializePlayer(Game game, string playerName);
        string PlayBotPlayerTurn(Game game);
    }
}