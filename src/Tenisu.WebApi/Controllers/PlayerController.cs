using Microsoft.AspNetCore.Mvc;
using Tenisu.Application.DTOs;
using Tenisu.Application.Interfaces;
using Tenisu.Application.Model;
namespace Tenisu.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{playerId:int}")]
        [ProducesResponseType<PlayerResponseDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PlayerResponseDTO>> GetPlayer([FromRoute] int playerId, CancellationToken cancellationToken)
        {
            var player = await _playerService.GetPlayerAsync(playerId, cancellationToken);

            if (player == null)
            {
                return NotFound($"The player with the id '{playerId}' has not been found");
            }

            return Ok(player);
        }

        [HttpGet("All")]
        [ProducesResponseType<IEnumerable<PlayerResponseDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PlayerResponseDTO>>> GetAllPlayers(CancellationToken cancellationToken)
        {
            var players = await _playerService.GetAllPlayersAsync(cancellationToken);
            return Ok(players);
        }

        [HttpGet("Page")]
        [ProducesResponseType<Page<PlayerResponseDTO>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<Page<PlayerResponseDTO>>> GetPageOfPlayers(int pageNum, int pageSize, CancellationToken cancellationToken)
        {
            var playersPage = await _playerService.GetPageOfPlayersAsync(pageNum, pageSize, cancellationToken);
            return Ok(playersPage);
        }

        [HttpPost]
        [ProducesResponseType<int>(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> PostPlayer([FromBody] PlayerDTO player, CancellationToken cancellationToken)
        {
            var returnedId = await _playerService.AddPlayerAsync(player, cancellationToken);
            return CreatedAtAction(nameof(GetPlayer), new { playerId = returnedId }, returnedId);
        }
    }
}
