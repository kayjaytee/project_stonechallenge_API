using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;
using RockPaperScissorsAPI.Validation;
using System;
using System.Security.Claims;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MatchController : ControllerBase
{
    private readonly IPlayerInviteService playerInviteService;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    internal static PlayerInvite playerInvite = new PlayerInvite();
    internal static Game game = new Game();
    internal static User user = new User();
    internal static PlayerMoves playerMoves = new PlayerMoves();

    public MatchController(IPlayerInviteService playerInviteService,
                            IGameService gameService,
                            IUserService userService,
                            IConfiguration configuration,
                            IHttpContextAccessor httpContextAccessor)
    {
        this.playerInviteService = playerInviteService;
        this.gameService = gameService;
        this.userService = userService;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }


    // f. Spara valda drag i spelet(Sparas i databasen)
    [HttpPut("Rock (1) Paper (2) Scissor (3)"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IActionResult> MakeMoveAsync(long gameID, short decideMove)
    {
        var myUserID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        playerMoves.MoveID = decideMove;
        
        var game = await gameService.FetchGameIDAsync(gameID);

        switch(game)
        {
            case null:
                return NotFound();

            case { PlayerWinner: not null }:
                return BadRequest("The game is already finished");

            case { PlayerOneID: var playerOneID,
                   PlayerTwoID: var playerTwoID }
                 when
                   playerOneID != myUserID &&
                   playerTwoID != myUserID:
                return BadRequest("You are not invited to this game.");
        }

        try
        {

            if (game.PlayerOneID == myUserID)
            {
                var result = await gameService.PlayerOneMove(game, playerMoves);

                return Ok("You picked: " + playerMoves.MoveID);
            }

            if (game.PlayerTwoID == myUserID)
            {
                var result = await gameService.PlayerTwoMove(game, playerMoves);
                return Ok("You picked: " + playerMoves.MoveID);
            }
            else
            {
                return BadRequest();
            }

           
        }
        catch
        {
            throw;
        }
    }

    private async Task<OkObjectResult> DetermineWinnerAndUpdateToDatabase(Game game, UserScore user)
    {
        var winner = await gameService.DetermineWinnerAsync(game);
        var updateScore = await gameService.IncreaseGamesPlayedAsync(game, user);

        return Ok(winner);
    }


    // h. iv. Färdigspelad
    [HttpPatch("Determine Winner of Game")]
    public async Task<IActionResult> DetermineWinnerOfGame(long gameID)
    {
        try
        {
            var game = await gameService.FetchGameIDAsync(gameID);
            game.GameID = gameID;

            if (game.IsFinished == true)
            {
                return Ok("Game is already finished");
            }

            UserScore user = new UserScore();

            switch (game.PlayerOneChoice) //Player 1
            {
                case 1: // Rock VS.
                    switch (game.PlayerTwoChoice) //Player 2
                    {
                        case 1: //Rock

                            var draw = await gameService.DetermineDrawAsync(game);
                            var updateScore = await gameService.IncreaseGamesPlayedAsync(game, user);
                            return Ok("DRAW");
                        case 2: //Paper

                            game.PlayerWinner = game.PlayerTwoID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 2 Wins");

                        case 3: //Scissors

                            game.PlayerWinner = game.PlayerOneID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 1 Wins");

                        default:
                            throw new ArgumentException("Error occured");
                    }
                
                case 2: //Paper VS.
                    switch (game.PlayerTwoChoice) //Player 2
                    {
                        case 1: //Rock

                            game.PlayerWinner = game.PlayerOneID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 1 Wins");

                        case 2: //Paper

                            var draw = await gameService.DetermineDrawAsync(game);
                            var updateScore = await gameService.IncreaseGamesPlayedAsync(game, user);
                            return Ok("DRAW");

                        case 3: //Scissors

                            game.PlayerWinner = game.PlayerTwoID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 2 Wins");

                        default:
                            throw new ArgumentException("Error occured");
                    }
                case 3: //Scissor VS.
                    switch (game.PlayerTwoChoice) //Player 2
                    {
                        case 1: //Rock

                            game.PlayerWinner = game.PlayerTwoID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 2 Wins");

                        case 2: //Paper

                            game.PlayerWinner = game.PlayerOneID;
                            await DetermineWinnerAndUpdateToDatabase(game, user);
                            return Ok("Player 1 Wins");

                        case 3: //Scissors

                            var draw = await gameService.DetermineDrawAsync(game);
                            var updateScore = await gameService.IncreaseGamesPlayedAsync(game, user);
                            return Ok("DRAW");

                        default:
                            throw new ArgumentException("Error occured");
                    }
                default:
                    throw new ArgumentException("Error occured");
            }
        }
        catch (Exception e)
        {
            throw new ArgumentException(e.Message, e.Source);
        }
    }


}
