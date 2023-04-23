using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/CRUD/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService gameService;

    public GameController(IGameService gameService) => this.gameService = gameService;




    [HttpGet("GetGamesList")]
    public async Task<List<Game>> GetGamesAsync()
    {
        try
        {
            return await gameService.GetGamesAsync();
        }
        catch
        {
            throw;
        }

    }

    [HttpGet("GetGameById")]
    public async Task<IEnumerable<Game>?> GetGameByIdAsync(int GameID)
    {
        try
        {
            var response = await gameService.GetGameByIdAsync(GameID);

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

    [HttpPost("CreateNewGame")]
    public async Task<IActionResult> CreateNewGameAsync(Game game)
    {
        if (game == null)
        {
            return BadRequest();
        }

        try
        {
            var response = await gameService.CreateNewGameAsync(game);

            return Ok(response);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("UpdateGame")]
    public async Task<IActionResult> UpdateGameAsync(Game game)
    {
        if (game == null)
        {
            return BadRequest();
        }

        try
        {
            var result = await gameService.UpdateGameAsync(game);
            return Ok(result);
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("DeleteGame")]
    public async Task<long> DeleteGameAsync(long GameID)
    {
        try
        {
            var response = await gameService.DeleteGameAsync(GameID);
            return response;
        }
        catch
        {
            throw;
        }
    }
}
