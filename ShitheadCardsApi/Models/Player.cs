using System.Collections.Generic;

namespace ShitheadCardsApi.Models
{
    public class Player
    {
        public Player()
        {
            InHandCards = new List<string>();
            DownCards = new List<string>();
            OpenCards = new List<string>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> InHandCards { get; set; }
        public List<string> DownCards { get; set; }
        public List<string> OpenCards { get; set; }
        public int Status { get; set; }
    }
}
