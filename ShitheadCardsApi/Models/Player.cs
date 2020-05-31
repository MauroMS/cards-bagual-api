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
            Status = StatusEnum.SETUP;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string LastDownCard { get; set; }
        public List<string> InHandCards { get; set; }
        public List<string> DownCards { get; set; }
        public List<string> OpenCards { get; set; }
        public StatusEnum Status { get; set; }
    }
}
