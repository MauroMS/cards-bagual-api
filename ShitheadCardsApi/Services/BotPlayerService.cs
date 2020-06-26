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

            botPlayer.OpenCards = handAndOpen.GetRange(0, 3);
            botPlayer.InHandCards = handAndOpen.GetRange(3, 3);

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

            return null;
        }

    }
}
