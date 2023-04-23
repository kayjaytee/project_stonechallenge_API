using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/CRUD/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService userService;

    public UserController(IUserService userService) => this.userService = userService;

    [HttpGet("GetUsersList")]
    public async Task<List<User>> GetUsersAsync()
    {
        try
        {
            return await userService.GetUsersAsync();
        }
        catch
        {
            throw;
        }

    }

    [HttpGet("GetUserById")]
    public async Task<IEnumerable<User>?> GetUserByIdAsync(long UserID)
    {
        try
        {
            var response = await userService.GetUserByIdAsync(UserID);

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
    
    [HttpPost("CreateNewUser")]
    public async Task<IActionResult> CreateNewUserAsync(User user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        try
        {
            var response = await userService.CreateNewUserAsync(user);

            return Ok(response);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUserAsync(User user)
    {
        if (user == null)
        {
            return BadRequest();
        }

        try
        {
            var result = await userService.UpdateUserAsync(user);
            return Ok(result);
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("DeleteUser")]
    public async Task<long> DeleteUserAsync(long UserID)
    {
        try
        {
            var response = await userService.DeleteUserAsync(UserID);
            return response;
        }
        catch
        {
            throw;
        }
    }


}
