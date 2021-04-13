using System.Collections.Generic;
using Bagual.Services.Shithead.Models;

namespace Bagual.Services.Shithead.Interfaces
{
    public interface IGameService
    {
        Game CreateOrJoinGame(string gameName, string playerName);
        Game GetGame(string gameName);
        Game SwitchPlayerCards(string gameName, string playerId, string openCard, string handCard);
        Game SetPlayerToStart(string gameName, string playerId);
        Game DiscardPlayerCards(string gameName, string playerId, string cards);
        Game MoveTableCardsToPlayer(string gameName, string playerId);
        List<Game> List(string nameFilter, bool filterOut = true);
        Game AddBotPlayer(string gameName);
        void PlayBotTurns();
    }
}
