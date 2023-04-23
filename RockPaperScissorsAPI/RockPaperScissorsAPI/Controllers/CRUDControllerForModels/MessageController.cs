using Microsoft.AspNetCore.Mvc;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Services;

namespace RockPaperScissorsAPI.Controllers;

[ApiController]
[Route("/api/CRUD/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService messageService;
    public MessageController(IMessageService messageService) => this.messageService = messageService;

    [HttpGet("GetMessagesList")]
    public async Task<List<Message>> GetMessagesAsync()
    {
        try
        {
            return await messageService.GetMessagesAsync();
        }
        catch
        {
            throw;
        }
    }
    
    [HttpGet("GetMessageById")]
    public async Task<IEnumerable<Message>?> GetMessageByIdAsync(long MessageID)
    {
        try
        {
            var response = await messageService.GetMessageByIdAsync(MessageID);

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

    [HttpPost("CreateNewMessage")]
    public async Task<IActionResult> CreateNewMessageAsync(Message message)
    {
        if (message == null)
        {
            return BadRequest();
        }

        try
        {
            var response = await messageService.CreateNewMessageAsync(message);

            return Ok(response);
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("UpdateMessage")]
    public async Task<IActionResult> UpdateMessageAsync(Message message)
    {
        if (message == null)
        {
            return BadRequest();
        }

        try
        {
            var result = await messageService.UpdateMessageAsync(message);
            return Ok(result);
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("DeleteMessage")]
    public async Task<long> DeleteMessageAsync(long MessageID)
    {
        try
        {
            var response = await messageService.DeleteMessageAsync(MessageID);
            return response;
        }
        catch
        {
            throw;
        }
    }
}
