using System;
namespace ShitheadCardsApi.Models
{
    public class GameException: Exception
    {
        public GameException(string message): base(message)
        {
           
        }
    }
}
