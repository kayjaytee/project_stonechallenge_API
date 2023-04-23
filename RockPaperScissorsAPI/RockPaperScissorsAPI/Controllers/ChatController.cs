using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;
using RockPaperScissorsAPI.Validation;
using System.Security.Claims;

namespace RockPaperScissorsAPI.Controllers;

public class ChatController : ControllerBase
{
    private readonly IMessageService messageService;
    private readonly IUserService userService;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    internal static Message message = new Message();
    internal static User user = new User();

    public ChatController(IMessageService messageService,
                          IUserService userService,
                          IConfiguration configuration,
                          IHttpContextAccessor httpContextAccessor)
    {
        this.messageService = messageService;
        this.userService = userService;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;

    }

    // c. Läsa meddelanden (läses från databasen)
    [HttpGet("ReadMyMessages"), Authorize(Policy = "Authenticated for Login")]
    public async Task<List<Message?>> GetMyMessagesAsync()
    {

        var userID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

        try
        {
            var response = await messageService.ReadMessagesAsync(userID);
       
            var fromUser = await userService.GetUserFromIDAsync(message.FromUserID);


            return response; //Needs to be customized to be prettier but it's working

        }
        catch
        {
            throw;
        }
    }

    // d.Spara meddelande(till databasen)
    [HttpPost("SendMessage"), Authorize(Policy = "Authenticated for Login")]
    public async Task<IActionResult> SendMessageAsync(SendMessage request)
    {
        

        try
        {


            //FromUserID
            message.FromUserID = long.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            //ToUserID
            var fetchID = await userService.FindIdFromUsernameAsync(request.ToUserName);
            message.ToUserID = (long)fetchID;


            if (message.ToUserID == message.FromUserID)
            {
                return BadRequest("You can't send messages to yourself!");
            }

            //The Message Text
            message.Value = request.Value;

            var response = await messageService.GenerateMessageAsync(message, request.ToUserName);


            return Ok("You sent message to: [" + request.ToUserName + "]: \nMessage: " + request.Value);

        }
        catch
        {
            throw;
        }

    }

    [HttpGet("Fetch My ID")]
    public async Task<IActionResult> FetchMyID(string username)
    {
        var userID = await userService.FindIdFromUsernameAsync(username);

        if (userID == null) { return NotFound(); }

        return Ok(new { userID });

    }



}
