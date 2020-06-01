using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShitheadCardsApi.Interfaces;
using ShitheadCardsApi.Models;
using ShitheadCardsApi.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

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
        public IActionResult Get()
        {
            return Ok("OK");
        }


        /// <summary>
        /// Creates or joins an existing Game, based on gameName
        /// </summary>
        /// <param name="gameName">Game name</param>
        /// <param name="playerName">Player Name</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/player/{playerName}")]
        public IActionResult GetCreateOrJoinGame(string gameName, string playerName)
        {
            try
            {
                Game game = _gameService.CreateOrJoinGame(gameName, playerName);
                return Ok(new CreateOrJoinGameResponse(game, playerName));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }


        /// <summary>
        /// Get current state of the game, with the data related to playerId
        /// </summary>
        /// <param name="gameName">Game Name</param>
        /// <param name="playerId">Player Id</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/{playerId}")]
        public IActionResult GetGame(string gameName, string playerId)
        {
            try
            {
                Game game = _gameService.GetGame(gameName);
                if (game == null)
                    throw new GameException("Game not found");
                return Ok(new GameResponse(game, playerId));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

        /// <summary>
        /// Move card from your hand to the open cards
        /// </summary>
        /// <param name="gameName">Game Name</param>
        /// <param name="playerId">Player Id</param>
        /// <param name="openCard">Open face up card</param>
        /// <param name="handCard">Hand Card</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/{playerId}/switch/{openCard}/{handCard}")]
        public IActionResult GetSwitchPlayerCard(string gameName, string playerId, string openCard, string handCard)
        {
            try
            {
                Game game = _gameService.SwitchPlayerCards(gameName, playerId, openCard, handCard);
                return Ok(new GameResponse(game, playerId));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

        /// <summary>
        /// Set player status from SETUP to PLAYING. And Updates the game status to playing, if all players are ready.
        /// </summary>
        /// <param name="gameName">Game Name</param>
        /// <param name="playerId">Player Id</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/{playerId}/start")]
        public IActionResult GetStartPlayer(string gameName, string playerId)
        {
            try
            {
                Game game = _gameService.SetPlayerToStart(gameName, playerId);
                return Ok(new GameResponse(game, playerId));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

        /// <summary>
        /// Draw table cards to player's hand.
        /// </summary>
        /// <param name="gameName">Game Name</param>
        /// <param name="playerId">Player Id</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/{playerId}/table")]
        public IActionResult GetTableCardsToPlayer(string gameName, string playerId)
        {
            try
            {
                Game game = _gameService.MoveTableCardsToPlayer(gameName, playerId);
                return Ok(new GameResponse(game, playerId));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

        /// <summary>
        /// Discard player's selected cards
        /// </summary>
        /// <param name="gameName">Game Name</param>
        /// <param name="playerId">Player Id</param>
        /// <param name="cards">Comma separated string with all cards the user is discarding</param>
        /// <returns>Current game state</returns>
        [HttpGet("game/{gameName}/{playerId}/discard/{cards}")]
        public IActionResult GetDiscardPlayerCard(string gameName, string playerId, string cards)
        {
            try
            {
                Game game = _gameService.DiscardPlayerCards(gameName, playerId, cards);
                return Ok(new GameResponse(game, playerId));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

        /// <summary>
        /// List games available for joining
        /// </summary>
        /// <param name="gameName">Optional Game Name filter</param>
        /// <returns>Current list of games for joining</returns>
        [HttpGet("game")]
        public IActionResult ListGames(string gameName)
        {
            try
            {
                List<Game> games = _gameService.List(gameName);
                return Ok(games.Select(g => new ListGameResponse(g)));
            }
            catch (GameException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
        }

    }
}
