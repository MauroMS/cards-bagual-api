﻿using System.Collections.Generic;
using Bagual.Services.Shithead.Models;

namespace Bagual.Services.Shithead.Interfaces
{
    public interface IShitheadService
    {
        List<string> CreateDeck();
        string ChooseFirstTurn(List<Player> players);
        string NextPlayerFrom(List<Player> players, string playerId, int skipPlayers);
        DiscardResult EvaluateCardsOnTable(List<string> cardsToBePlayed, List<string> tableCards);
        bool SameNumber(string card1, string card2);
        bool CanCardGoOn(string cardNumber, string lastTableCardNumber);
        int GetPlayerLowestCard(List<string> cards, int minVal = 4);
        string GetLastTableCardNotThree(List<string> tableCards);
        int GetNumericValueFromCard(string card);
    }
}
