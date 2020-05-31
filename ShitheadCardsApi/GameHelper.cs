using ShitheadCardsApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace ShitheadCardsApi
{
    public static class GameHelper
    {
        /// <summary>
        /// Sort players in the correct order
        /// </summary>
        /// <param name="players">List of all players</param>
        /// <param name="currentPlayerIndex">Current player index</param>
        /// <param name="rightPlayer">Amount of players on right side of the board</param>
        /// <param name="leftPlayer">Amount of players on left side of the board</param>
        /// <param name="sortedPlayers">List of sorted players</param>
        /// <returns>Sorted players list</returns>
        private static List<Player> GetOtherPlayersInternal(List<Player> players, int currentPlayerIndex, int rightPlayer, int leftPlayer, List<Player> sortedPlayers)
        {
            int expectedPlayerCount = players.Count - 1;
            if (sortedPlayers.Count == expectedPlayerCount)
                return sortedPlayers;

            if (leftPlayer < 0)
            {
                int endOfListIndex = currentPlayerIndex + leftPlayer;
                if (endOfListIndex < 0)
                    endOfListIndex = players.Count + endOfListIndex;
                if (endOfListIndex == currentPlayerIndex)
                    sortedPlayers.Add(players[endOfListIndex - 1]);
                else
                    sortedPlayers.Add(players[endOfListIndex]);
                return GetOtherPlayersInternal(players, currentPlayerIndex, rightPlayer, ++leftPlayer, sortedPlayers);
            }
            else
            {
                if (rightPlayer > expectedPlayerCount)
                    rightPlayer -= players.Count;

                if (rightPlayer < 0)
                    rightPlayer = players.Count + rightPlayer;

                if (rightPlayer != currentPlayerIndex)
                    sortedPlayers.Add(players[rightPlayer]);


                return GetOtherPlayersInternal(players, currentPlayerIndex, --rightPlayer, leftPlayer, sortedPlayers);
            }
        }

        /// <summary>
        /// Get all sorted players in the correct order
        /// </summary>
        /// <param name="players">List of all players</param>
        /// <param name="playerdId">Player Id</param>
        /// <returns>List of sorted players</returns>
        public static List<Player> GetOtherPlayers(List<Player> players, string playerdId)
        {
            var currentIndex = players.FindIndex(p => p.Id == playerdId);

            List<Player> sortedPlayers = new List<Player>();
            if (players.Count == 2)
                sortedPlayers = GetOtherPlayersInternal(players, currentIndex, 0, -1, sortedPlayers);
            else
                sortedPlayers = GetOtherPlayersInternal(players, currentIndex, currentIndex + ((players.Count - 1) - 2), -2, sortedPlayers);  // (players.Count - 1 - 2) :: (players.Count - 1) is to get the actual position on the array, and -2 is for the 2 playes on the left side that were already added.  



            return sortedPlayers;
        }
    }
}
