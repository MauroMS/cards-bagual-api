﻿using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShitheadCardsApi
{
    public class ShitheadService : IShitheadService
    {
        private static Random rng = new Random();
        private static string[] suits = { "H", "D", "C", "S" };
        private static string[] numbers = { "2", "3", "4", "5", "6", "7", "8", "9", "0", "J", "Q", "K", "A" };

        private static int[] numbersValue = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

        public ShitheadService()
        {

        }

        public string ChooseFirstTurn(List<Player> players)
        {
            KeyValuePair<string, int> starterPlayer = new KeyValuePair<string, int>();

            foreach (var player in players)
            {
                var lowestCard = GetPlayerLowestCard(player.InHandCards);

                if (lowestCard == 4)
                    return player.Name;

                if (starterPlayer.Value == 0 || starterPlayer.Value > lowestCard)
                    starterPlayer = new KeyValuePair<string, int>(player.Name, lowestCard);
            }

            return starterPlayer.Key;
        }

        private int GetPlayerLowestCard(List<string> cards)
        {
            return cards.Select(card => GetNumericValue(GetCardNumber(card))).Where(cardValue => cardValue >= 4).Min();
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

        public DiscardResult EvaluateCardsOnTable(List<string> cardsToBePlayed, List<string> tableCards)
        {
            string lastTableCardNotThree = tableCards.FindLast(tc => GetCardNumber(tc) != "3");

            if (tableCards.Count == 0 || lastTableCardNotThree == null)
            {
                if ((cardsToBePlayed.Count == 4) || (GetCardNumber(cardsToBePlayed[0]) == "0"))
                {
                    return DiscardResult.OkBurned;
                }
                return DiscardResult.Ok;
            }

            string cardNumber = GetCardNumber(cardsToBePlayed[0]);

            string lastTableCardNumber = GetCardNumber(lastTableCardNotThree);

            bool acceptDiscard = CanCardGoOn(cardNumber, lastTableCardNumber);

            if (!acceptDiscard)
                return DiscardResult.Refuse;

            List<string> cardsInARow = new List<string>(cardsToBePlayed);
            cardsInARow.AddRange(tableCards.TakeLast(4).Reverse());

            // 10 burns
            if (cardNumber == "0")
            {
                return DiscardResult.OkBurned;
            }

            // 4 of the same in a row burns
            if (cardsInARow.Count >= 4 && cardsInARow.Take(4).Select(c => GetCardNumber(c)).Distinct().Count() == 1)
            {
                return DiscardResult.OkBurned;
            }

            return DiscardResult.Ok;
        }

        private bool CanCardGoOn(string cardNumber, string lastTableCardNumber)
        {
            if (lastTableCardNumber == null)
            {
                return true;
            }

            int toPut = GetNumericValue(cardNumber);

            if (toPut == 10 || toPut == 2 || toPut == 3)
                return true;

            int toAccept = GetNumericValue(lastTableCardNumber);

            if (toPut == toAccept)
                return true;

            if (toAccept == 7)
            {
                return toPut < toAccept;
            }

            return toPut > toAccept;
        }

        public static int GetNumericValue(string cardNumber)
        {
            return numbersValue[Array.IndexOf(numbers, cardNumber)];
        }

        public static string GetCardNumber(string card)
        {
            return card.Substring(0,1);
        }

        public string NextPlayerFrom(List<Player> players, string playerId, int step)
        {
            int numPlayersPlaying = players.FindAll(p => p.Status == StatusEnum.PLAYING).Count;

            int playerIdIndex = players.FindIndex(p => p.Id == playerId);

            if (step >= numPlayersPlaying && players[playerIdIndex].Status == StatusEnum.PLAYING)
                return players[playerIdIndex].Name;

            int nextTurnIndex = -1;
            do
            {
                nextTurnIndex = (playerIdIndex + step++) % players.Count;
            }
            while (players[nextTurnIndex].Status != StatusEnum.PLAYING);

            return players[nextTurnIndex].Name;
            
        }

        public bool SameNumber(string card1, string card2)
        {
            return GetCardNumber(card1) == GetCardNumber(card2);
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
