using ShitheadCardsApi.Models;
using System.Collections.Generic;

namespace ShitheadCardsApi
{
    public static class GameHelper
    {
        public static List<Player> GetOtherPlayers(List<Player> players, int currentPlayerIndex, int positiveSeed, int nextPlayer, List<Player> sortedPlayers)
        {
            int expectedPlayerCount = players.Count - 1;
            if (sortedPlayers.Count == expectedPlayerCount)
                return sortedPlayers;

            if (nextPlayer < 0)
            {
                int endOfListIndex = currentPlayerIndex + nextPlayer;
                if (endOfListIndex < 0)
                    endOfListIndex = players.Count + endOfListIndex;
                if (endOfListIndex == currentPlayerIndex)
                    sortedPlayers.Add(players[endOfListIndex - 1]);
                else
                    sortedPlayers.Add(players[endOfListIndex]);
                return GetOtherPlayers(players, currentPlayerIndex, positiveSeed, ++nextPlayer, sortedPlayers);
            }

            

            //if (nextPlayer == 0)
            //{
            //    if (positiveSeed > 0)
            //        return GetOtherPlayers(players, currentPlayerIndex, positiveSeed - 1, currentPlayerIndex + positiveSeed, sortedPlayers);

            //    sortedPlayers.Add(players[nextPlayer]);
            //}

            //if (nextPlayer > 0)
            //{
            //    if (nextPlayer >= expectedPlayerCount)
            //        nextPlayer = 0 + positiveSeed;

            //    if (positiveSeed == 0)
            //        sortedPlayers.Add(players[nextPlayer]);
            //    else
            //        return GetOtherPlayers(players, currentPlayerIndex, --positiveSeed, ++nextPlayer, sortedPlayers);
            //}

            return sortedPlayers;
        }
    }
}
