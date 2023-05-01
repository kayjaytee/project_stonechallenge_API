using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;
using System.Security.Claims;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class InviteController : ControllerBase
{
    private readonly IPlayerInviteService playerInviteService;
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    internal static PlayerInvite playerInvite = new PlayerInvite();
    internal static Game game = new Game();
    internal static User user = new User();

    public InviteController(IPlayerInviteService playerInviteService,
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
    /// <summary>
    /// h. Läsa spel drag och dess status (läses från databasen)
    ///   i.Inbjudan skickad
    ///   ii.Pågående
    ///  iii.Avvisad inbjudan
    ///   iv. Färdigspelad
    /// </summary>
    /// <returns></returns>
    /// 
    [HttpGet("CheckAnyRequestsToPlay"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IEnumerable<string>> CheckRequestsAsync()
    {
        var userID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        playerInvite.InviteToUserID = userID;

        try
        {
            var invitesFrom = await playerInviteService.GetPlayerInviteToUserIDAsync(userID);
            var messages = new List<string>();
            foreach (var request in invitesFrom)
            {
                var username = await userService.GetUsernameFromIdAsync(request.InviteFromUserID);
                messages.Add($"[{username}] has challenged you to a game of Rock Paper Scissors! Accept the invite to start the game!");
            }

            return messages;
        }
        catch
        {
            throw;
        }

    }

    // e. Spara inbjudan till spel (till databasen)
    [HttpPost("InvitePlayerToGame"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IActionResult> InviteToGameAsync(string username)
    {
        var userID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        playerInvite.InviteFromUserID = userID;
        try
        {
            var getUserName = await userService.FindIdFromUsernameAsync(username);
            if (getUserName == null)
            {
                return BadRequest("Player could not be found.");
            }
            playerInvite.InviteToUserID = (long)getUserName;
            var invite = await playerInviteService.InvitePlayerAsync(playerInvite);

            return Ok("You invited " + username + " to a game of Rock Paper Scissors!");
        }
        catch (SqlException ex) when (ex.Number == 2627) // Customized SqlException to handle error when primary keys are duplicate.
        {
            return BadRequest($"A request to invite {username} has already been sent.");
        }
        catch
        {
            return BadRequest("Player could not be found.");
        }
    }

    [HttpGet("AcceptInviteFromUser"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IActionResult> AcceptInviteFromUsernameAsync(string username)
    {
        var myUserID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        playerInvite.InviteToUserID = myUserID;

        try
        {
            var opponentUserID = await userService.FindIdFromUsernameAsync(username);
            if (opponentUserID == null)
            {
                return BadRequest("Request denied.");
            }

            playerInvite.InviteFromUserID = (long)opponentUserID;

            var invite = await playerInviteService.FindInvitationAsync(playerInvite.InviteFromUserID, playerInvite.InviteToUserID);
            if (invite == null)
            {
                return BadRequest("Request denied.");
            }

            var accept = await playerInviteService.AcceptInviteAsync(playerInvite);
            if (accept == null)
            {
                return BadRequest("Request denied");
            }
            else
            {
                var createGame = await gameService.HostNewGameAsync(game, playerInvite);
                var gameID = await gameService.FetchNewlyCreatedGameIDAsync(game.GameID);
                game.GameID = gameID;
                var removeInvite = await playerInviteService.DeletePlayerInvitationAsync(playerInvite.InviteFromUserID);
                return Ok("You accepted " + username + " challenge to a game of Rock Paper Scissors! \nThe Game ID is: " + game.GameID);
            }

        }
        catch (NullReferenceException)
        {
            return BadRequest("Request denied.");
        }
        catch
        {
            return BadRequest("Request denied.");
        }
    }

    [HttpPost("DeclineInviteFromUser"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IActionResult> DeclinePlayerInviteFromUsernameAsync(string username)
    {
        var userID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);


        playerInvite.InviteToUserID = userID;

        try
        {
            var getUserName = await userService.FindIdFromUsernameAsync(username);
            if (getUserName == null)
            {
                return BadRequest("Request denied.");
            }

            playerInvite.InviteFromUserID = (long)getUserName;

            var response = await playerInviteService.DeletePlayerInvitationAsync(playerInvite.InviteFromUserID);

            return Ok("You declined " + username + " challenge to a game of Rock Paper Scissors.");
        }
        catch
        {
            throw;
        }
    }
}
