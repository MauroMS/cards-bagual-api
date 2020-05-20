using System.Collections.Generic;

namespace ShitheadCardsApi.Models
{
    public class GameResponse
    {
        public GameResponse(Game game, string playerId)
        {
            this.Name = game.Name;
            this.Status = game.Status;
            this.DeckCount = game.CardsInDeck.Count;
            this.LastBurnedCard = game.LastBurnedCard;
            this.BurnedCardsCount = game.BurnedCardsCount;
            this.TableCards = game.TableCards;
            this.PlayerNameTurn = game.PlayerNameTurn;

            this.Players = game.Players.FindAll(gp => gp.Id != playerId).ConvertAll(op => new PlayerOtherResponse(op));
            this.MySelf = new PlayerMyselfResponse(game.Players.Find(gp => gp.Id == playerId));
        }

        public string Name { get; set; }
        public StatusEnum Status { get; set; }
        public List<PlayerOtherResponse> Players { get; set; }
        public PlayerMyselfResponse MySelf { get; set; }
        public List<string> TableCards { get; set; }
        public string LastBurnedCard { get; set; }
        public int BurnedCardsCount { get; set; }
        public int DeckCount { get; set; }
        public string PlayerNameTurn { get; set; }
    }

    public class PlayerOtherResponse
    {
        public PlayerOtherResponse (Player player)
        {
            this.Name = player.Name;
            this.HandCount = player.InHandCards.Count;
            this.DownCount = player.DownCards.Count;
            this.OpenCards = player.OpenCards;
            this.Status = player.Status;
        }
        public string Name { get; set; }
        public int HandCount { get; set; }
        public int DownCount { get; set; }
        public List<string> OpenCards { get; set; }
        public StatusEnum Status { get; set; }
    }


    public class PlayerMyselfResponse
    {
        public PlayerMyselfResponse(Player player)
        {
            if (player == null)
                return;

            this.Name = player.Name;
            this.HandCards = player.InHandCards;
            this.DownCount = player.DownCards.Count;
            this.OpenCards = player.OpenCards;
            this.Status = player.Status;
        }

        public string Name { get; set; }
        public int DownCount { get; set; }
        public List<string> HandCards { get; set; }
        public List<string> OpenCards { get; set; }
        public StatusEnum Status { get; set; }
    }
}
