using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using ShitheadCardsApi.ViewModels;

namespace ShitheadCardsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShitheadController : ControllerBase
    {

        private readonly ILogger<ShitheadController> _logger;
        private readonly IGameService _gameService;

        public ShitheadController(ILogger<ShitheadController> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        [HttpGet("")]
        public string Get()
        {
            return "OK";
        }


        [HttpGet("game/{gameName}/player/{playerName}")]
        public CreateOrJoinGameResponse GetCreateOrJoinGame(string gameName, string playerName)
        {
            Game game = _gameService.CreateOrJoinGame(gameName, playerName);

            return new CreateOrJoinGameResponse(game, playerName);
        }


        [HttpGet("game/{gameName}/{playerId}")]
        public GameResponse GetGame(string gameName, string playerId)
        {
            Game game = _gameService.GetGame(gameName);
            return new GameResponse(game, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/switch/{openCard}/{handCard}")]
        public GameResponse GetSwitchPlayerCard(string gameName, string playerId, string openCard, string handCard)
        {
            Game game = _gameService.SwitchPlayerCards(gameName, playerId, openCard, handCard);
            return new GameResponse(game, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/start")]
        public GameResponse GetStartPlayer(string gameName, string playerId)
        {
            Game game = _gameService.SetPlayerToStart(gameName, playerId);
            return new GameResponse(game, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/table")]
        public GameResponse GetTableCardsToPlayer(string gameName, string playerId)
        {
            Game game = _gameService.MoveTableCardsToPlayer(gameName, playerId);
            return new GameResponse(game, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/discard/{cards}")]
        public GameResponse GetDiscardPlayerCard(string gameName, string playerId, string cards)
        {
            Game game = _gameService.DiscardPlayerCards(gameName, playerId, cards);
            return new GameResponse(game, playerId);
        }

    }
}
