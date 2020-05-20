using System.Collections.Generic;
using ShitheadCardsApi.Models;

namespace ShitheadCardsApi.Interfaces
{
    public interface IShitheadService
    {
        List<string> CreateDeck();
        string chooseFirstTurn(List<Player> players);
    }
}
