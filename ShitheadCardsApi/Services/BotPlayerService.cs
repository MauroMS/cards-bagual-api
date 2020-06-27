using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShitheadCardsApi.DataContext;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShitheadCardsApi
{
    public class BotPlayerService : IBotPlayerService
    {
        private static string[] botPlayerNames = { "John", "Paul", "George", "Ringo" };

        private static string[] bestCards = { "4", "5", "6", "7", "8", "9", "J", "Q", "K", "A", "2", "3", "0" };

        private IShitheadService _shitheadService;

        public BotPlayerService(IShitheadService shitheadService)
        {
            _shitheadService = shitheadService;
        }

        public string GeneratePlayerName(Game game)
        {
            foreach(var pn in botPlayerNames)
            {
                if (!game.Players.Exists(p => p.Name.Equals(pn)))
                    return pn;
            }
            return "Elvis";
        }

        public void InitializePlayer(Game game, string playerName)
        {
            Player botPlayer = game.Players.Where(p => p.Name.Equals(playerName)).First();

            List<string> handAndOpen = botPlayer.InHandCards.Union(botPlayer.OpenCards).ToList();

            handAndOpen.Sort(CompareBestCard);

            botPlayer.OpenCards = handAndOpen.GetRange(3, 3);
            botPlayer.InHandCards = handAndOpen.GetRange(0, 3);

            game.Players.First(p => p.Name == playerName).Status = StatusEnum.PLAYING;
        }

        public static int CompareBestCard(string card1, string card2)
        {
            int c1pos = Array.IndexOf(bestCards, card1.Substring(0, 1));
            int c2pos = Array.IndexOf(bestCards, card2.Substring(0, 1));

            return c1pos - c2pos;
        }

        public string PlayBotPlayerTurn(Game game)
        {
            Player botPlayer = game.Players.Where(p => p.Name.Equals(game.PlayerNameTurn)).First();

            List<string> cardsToPlay;

            if (botPlayer.InHandCards.Count > 0)
            {
                cardsToPlay = botPlayer.InHandCards;
            }
            else if (botPlayer.OpenCards.Count > 0)
            {
                cardsToPlay = botPlayer.OpenCards;
            }
            else if (botPlayer.DownCards.Count > 0)
            { 
                return "down";
            }
            else
            {
                return null;
            }

            string lastTableCard = _shitheadService.GetLastTableCardNotThree(game.TableCards);

            // no card on table
            if (lastTableCard == null)
            {
                int playLowest = _shitheadService.GetPlayerLowestCard(cardsToPlay);
                if (playLowest > 0 )
                    return JoinThrow(cardsToPlay, playLowest);
            }

            int lastTableCardVal = _shitheadService.GetNumericValueFromCard(lastTableCard);

            // 2 on table
            if (lastTableCardVal == 2)
            {
                int playLowest = _shitheadService.GetPlayerLowestCard(cardsToPlay);
                if (playLowest > 0)
                    return JoinThrow(cardsToPlay, playLowest);
            }

            // same card if not 3 or 10
            if (lastTableCardVal != 3 && lastTableCardVal != 10 &&
                cardsToPlay.Exists(c => _shitheadService.GetNumericValueFromCard(c) == lastTableCardVal))
            {
                return JoinThrow(cardsToPlay, lastTableCardVal);
            }

            // 7 on table
            if (lastTableCardVal == 7)
            {
                int playLowest = _shitheadService.GetPlayerLowestCard(cardsToPlay);
                if (playLowest <= 7 && playLowest > 0)
                {
                    return JoinThrow(cardsToPlay, playLowest);
                }
            }
            // higher than on table
            else 
            {
                int playLowestHigherThan = _shitheadService.GetPlayerLowestCard(cardsToPlay, lastTableCardVal);
                if (playLowestHigherThan > 0 && playLowestHigherThan != 2 && playLowestHigherThan != 3 && playLowestHigherThan != 10)
                {
                    return JoinThrow(cardsToPlay, playLowestHigherThan);
                }
            }

            // check 3, if existent throw only 1
            if (cardsToPlay.Exists(c => _shitheadService.GetNumericValueFromCard(c) == 3))
            {
                return cardsToPlay.First(c => _shitheadService.GetNumericValueFromCard(c) == 3);
            }

            // check 2, if existent throw only 1
            if (cardsToPlay.Exists(c => _shitheadService.GetNumericValueFromCard(c) == 2))
            {
                return cardsToPlay.First(c => _shitheadService.GetNumericValueFromCard(c) == 2);
            }

            // check 10, if existent throw only 1
            if (cardsToPlay.Exists(c => _shitheadService.GetNumericValueFromCard(c) == 10))
            {
                return cardsToPlay.First(c => _shitheadService.GetNumericValueFromCard(c) == 10);
            }

            // can't put anything, return null to pick up the table
            return null;
        }

        public string JoinThrow(List<string> cardsToPlay, int cardVal)
        {
            return string.Join(",", cardsToPlay.Where(c => _shitheadService.GetNumericValueFromCard(c) == cardVal));
        }
    }

  
}
