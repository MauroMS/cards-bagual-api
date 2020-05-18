using ShitheadCardsApi.Interfaces;
using System;
using System.Collections.Generic;

namespace ShitheadCardsApi
{
    public class ShitheadService : IShitheadService
    {
        private static Random rng = new Random();
        private static string[] suits = { "H", "D", "C", "S" };
        private static string[] numbers = { "2", "3", "4", "5", "6", "7", "8", "9", "0", "J", "Q", "K", "A" };

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
