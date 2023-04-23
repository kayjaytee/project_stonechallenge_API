using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;


namespace RockPaperScissorsAPI.Controllers
{

    [ApiController]
    [Route("/api/CRUD/[controller]")]
    public class PlayerMovesController : ControllerBase
    {
        private readonly IPlayerMovesService playerMovesService;

        public PlayerMovesController(IPlayerMovesService playerMovesService) =>
            this.playerMovesService = playerMovesService;

        [HttpGet("GetPlayerMoves")]
        public async Task<List<PlayerMoves>> GetPlayerMovesAsync()
        {
            try
            {
                return await playerMovesService.GetPlayerMovesAsync();
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetPlayerMovesByID")]
        public async Task<IEnumerable<PlayerMoves>?> GetPlayerMovesByIDAsync(byte MoveID)
        {
            try
            {
                var response = await playerMovesService.GetPlayerMovesByIDAsync(MoveID);

                if (response == null)
                {
                    return null;
                }

                return response;
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("CreatePlayerMove")]
        public async Task<IActionResult> CreatePlayerMoveAsync(PlayerMoves playerMoves)
        {
            if (playerMoves == null)
            {
                return BadRequest();
            }

            try
            {
                var response = await playerMovesService.CreatePlayerMoveAsync(playerMoves);

                return Ok(response);
            }
            catch
            {
                throw;
            }
        }

        [HttpPut("UpdatePlayerMove")]
        public async Task<IActionResult> UpdatePlayerMoveAsync(PlayerMoves playerMoves)
        {
            if (playerMoves == null)
            {
                return BadRequest();
            }

            try
            {
                var result = await playerMovesService.UpdatePlayerMoveAsync(playerMoves);
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }

        [HttpDelete("")]
        public async Task<long> DeletePlayerMoveAsync(byte MoveID)
        {
            try
            {
                var response = await playerMovesService.DeletePlayerMoveAsync(MoveID);
                return response;
            }
            catch
            {
                throw;
            }
        }







    }
}
