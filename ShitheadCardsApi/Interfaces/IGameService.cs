using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.Interfaces
{
    public interface IGameService
    {
        Game CreateOrJoinGame(string gameName, string playerName);
        Game GetGame(string gameName);
        Game SwitchPlayerCards(string gameName, string playerId, string openCard, string handCard);
        Game SetPlayerToStart(string gameName, object playerId);
        Game DiscardPlayerCards(string gameName, string playerId, string cards);
        Game MoveTableCardsToPlayer(string gameName, string playerId);
    }
}
