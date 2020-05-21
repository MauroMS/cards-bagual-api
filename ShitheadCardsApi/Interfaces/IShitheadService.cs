using System.Collections.Generic;
using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.Interfaces
{
    public interface IShitheadService
    {
        List<string> CreateDeck();
        string ChooseFirstTurn(List<Player> players);
        string NextPlayerFrom(List<Player> players, string playerId, int skipPlayers);
        DiscardResult EvaluateCardsOnTable(List<string> cardsToBePlayed, List<string> tableCards);
        bool SameNumber(string card1, string card2);
        string GetCardNumber(string card);
    }
}
