using ShitheadCardsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShitheadCardsApi.Interfaces
{
    public interface IGameService
    {
        void CreateGame();

        Game GetGame(string name);
    }
}
