using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;

namespace ShitheadCardsApi
{
    public class ShitheadService : IShitheadService
    {
        private static Random rng = new Random();
        private static string[] suits = { "H", "D", "C", "S" };
        private static string[] numbers = { "2", "3", "4", "5", "6", "7", "8", "9", "0", "J", "Q", "K", "A" };

        public string ChooseFirstTurn(List<Player> players)
        {
            return players[rng.Next(0, players.Count - 1)].Id;
        }

        public List<string> CreateDeck()
        {
            List<string> deck = new List<string>();
            foreach (string suit in suits)
            {
                foreach (string number in numbers)
                {
                    deck.Add(number + suit);
                }
            }

            Shuffle(deck);

            return deck;
        }

        public string NextPlayerFrom(List<Player> players, string playerId)
        {
            List<Player> playersPlaying = players.FindAll(p => p.Status == StatusEnum.PLAYING);

            int playerIdIndex = playersPlaying.FindIndex(p => p.Id == playerId);

            if (playerIdIndex == (playersPlaying.Count -1) )
            {
                return playersPlaying[0].Name;
            }
            else
            {
                return playersPlaying[playerIdIndex+1].Name;
            }
        }

        private void Shuffle(List<string> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
