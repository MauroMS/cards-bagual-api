using ShitheadCardsApi.Models;
using System.Collections.Generic;

namespace ShitheadCardsApi
{
    public static class GameHelper
    {
        public static List<Player> GetOtherPlayersInternal(List<Player> players, int currentPlayerIndex, int rightPlayers, int leftPlayers, List<Player> sortedPlayers)
        {
            int expectedPlayerCount = players.Count - 1;
            if (sortedPlayers.Count == expectedPlayerCount)
                return sortedPlayers;

            if (leftPlayers < 0)
            {
                int endOfListIndex = currentPlayerIndex + leftPlayers;
                if (endOfListIndex < 0)
                    endOfListIndex = players.Count + endOfListIndex;
                if (endOfListIndex == currentPlayerIndex)
                    sortedPlayers.Add(players[endOfListIndex - 1]);
                else
                    sortedPlayers.Add(players[endOfListIndex]);
                return GetOtherPlayersInternal(players, currentPlayerIndex, rightPlayers, ++leftPlayers, sortedPlayers);
            }
            else
            {
                //int beginingOfListIndex = currentPlayerIndex - positiveSeed;
                //if (beginingOfListIndex < 0)
                //    rightPlayers = beginingOfListIndex - players.Count;
                //if (beginingOfListIndex == currentPlayerIndex)
                //    sortedPlayers.Add(players[beginingOfListIndex + 1]);
                //else
                //    sortedPlayers.Add(players[beginingOfListIndex]);
                //return GetOtherPlayersInternal(players, currentPlayerIndex, --rightPlayers, 0, sortedPlayers);

                if (rightPlayers > expectedPlayerCount)
                    rightPlayers = rightPlayers - expectedPlayerCount;

                if (rightPlayers < 0)
                    rightPlayers = players.Count + rightPlayers;

                if (rightPlayers != currentPlayerIndex && !sortedPlayers.Contains(players[rightPlayers]))
                    sortedPlayers.Add(players[rightPlayers]);


                return GetOtherPlayersInternal(players, currentPlayerIndex, --rightPlayers, 0, sortedPlayers);
            }
        }

        public static List<Player> GetOtherPlayers(List<Player> players, int currentIndex)
        {
            List<Player> sortedPlayers = new List<Player>();
            if (players.Count == 2)
                sortedPlayers = GetOtherPlayersInternal(players, currentIndex, 1, -1, sortedPlayers);
            else
                sortedPlayers = GetOtherPlayersInternal(players, currentIndex, currentIndex + 2, -2, sortedPlayers);

            return sortedPlayers;
        }
    }
}
