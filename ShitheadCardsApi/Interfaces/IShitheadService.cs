using System.Collections.Generic;
using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.Interfaces
{
    public interface IShitheadService
    {
        List<string> CreateDeck();
        string ChooseFirstTurn(List<Player> players);
        string NextPlayerFrom(List<Player> players, string playerId);
    }
}
