using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Services;

public interface IMessageService
{
    public Task<List<Message>> GetMessagesAsync();
    public Task<IEnumerable<Message>> GetMessageByIdAsync(long MessageID);
    public Task<long> CreateNewMessageAsync(Message message);
    public Task<long> UpdateMessageAsync(Message message);
    public Task<long> DeleteMessageAsync(long MessageID);
    public Task<long> GenerateMessageAsync(Message message, string username);
    public Task<List<Message?>> ReadMessagesAsync(long userID);
    
}