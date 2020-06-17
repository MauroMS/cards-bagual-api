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
            // TODO: add logic here to set "good" hand cards switching with open ones
            game.Players.First(p => p.Name == playerName).Status = StatusEnum.PLAYING;
        }

        public string PlayBotPlayerTurn(Game game)
        {
            // TODO: implement logic here for bot playing on which card to discard. "null" to collect the table
            return null;
        }

    }
}
