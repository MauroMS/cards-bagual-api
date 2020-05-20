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
        public Game GetSwitchPlayerCard(string gameName, string playerId, string openCard, string handCard)
        {  
            return _gameService.SwitchPlayerCards(gameName, playerId, openCard, handCard);
        }

        [HttpGet("game/{gameName}/{playerId}/start")]
        public Game GetStartPlayer(string gameName, string playerId)
        {
            return _gameService.SetPlayerToStart(gameName, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/table")]
        public Game GetTableCardsToPlayer(string gameName, string playerId)
        {
            return _gameService.MoveTableCardsToPlayer(gameName, playerId);
        }

        [HttpGet("game/{gameName}/{playerId}/discard/{cards}")]
        public Game GetDiscardPlayerCard(string gameName, string playerId, string cards)
        {
            return _gameService.DiscardPlayerCards(gameName, playerId, cards);
        }

    }
}
