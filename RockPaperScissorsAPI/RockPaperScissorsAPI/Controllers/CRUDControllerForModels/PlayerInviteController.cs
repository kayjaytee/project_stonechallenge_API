using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/CRUD/[controller]")]
public class PlayerInviteController : ControllerBase
{
    private readonly IPlayerInviteService playerInviteService;

    public PlayerInviteController(IPlayerInviteService playerInviteService) => this.playerInviteService = playerInviteService;

    [HttpGet("GetAllPlayerInvitesList")]
    public async Task<List<PlayerInvite>> GetAllPlayerInvitesAsync()
    {
        try
        {
            return await playerInviteService.GetAllPlayerInvitesAsync();
        }
        catch
        {
            throw;
        }

    }

    [HttpGet("GetPlayerInviteFromUserID")]
    public async Task<IEnumerable<PlayerInvite>?> GetPlayerInviteFromUserIDAsync(long InviteFromUserID)
    {
        try
        {
            var response = await playerInviteService.GetPlayerInviteFromUserIDAsync(InviteFromUserID);

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

    [HttpGet("GetPlayerInviteToUserID")]
    public async Task<IEnumerable<PlayerInvite>?> GetPlayerInviteToUserIDAsync(long InviteToUserID)
    {
        try
        {
            var response = await playerInviteService.GetPlayerInviteToUserIDAsync(InviteToUserID);

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

    [HttpPost("CreatePlayerInvitation")]
    public async Task<IActionResult> CreatePlayerInvitationAsync(PlayerInvite playerInvite)
    {
        if (playerInvite == null)
        {
            return BadRequest();
        }

        try
        {
            var response = await playerInviteService.CreatePlayerInvitationAsync(playerInvite);

            return Ok(response);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("EditPlayerInvitation")]
    public async Task<IActionResult> EditPlayerInvitationAsync(PlayerInvite playerInvite)
    {
        if (playerInvite == null)
        {
            return BadRequest();
        }

        try
        {
            var result = await playerInviteService.EditPlayerInvitationAsync(playerInvite);
            return Ok(result);
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("DeletePlayerInvitation")]
    public async Task<long> DeletePlayerInvitationAsync(long InviteFromUserID)
    {
        try
        {
            var response = await playerInviteService.DeletePlayerInvitationAsync(InviteFromUserID);
            return response;
        }
        catch
        {
            throw;
        }
    }

}
