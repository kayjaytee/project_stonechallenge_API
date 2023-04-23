using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Model;

namespace RockPaperScissorsAPI.Services;

public class MessageService : IMessageService
{
    private readonly DataContext _context;
    public MessageService(DataContext context) => _context = context;

    public async Task<List<Message>> GetMessagesAsync()
    {
        return await _context.Message
        .FromSqlRaw("[Procedure_GetMessages]")
        .ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetMessageByIdAsync(long MessageID)
    {
        var parameter = new SqlParameter("@MessageID", MessageID);

        var info = await Task.Run(() =>
        _context.Message
        .FromSqlRaw(@"EXECUTE [Procedure_GetMessageByID] @MessageID", parameter)
        .ToListAsync());

        return info;
    }

    public async Task<long> CreateNewMessageAsync(Message message)
    {

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@FromUserID", message.FromUserID));
        parameter.Add(new SqlParameter("@ToUserID", message.ToUserID));
        parameter.Add(new SqlParameter("@Value", message.Value));
        parameter.Add(new SqlParameter("@TimeSent", DateTime.Now.ToString()));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreateNewMessage]
            @FromUserID,
            @ToUserID,
            @Value,
            @TimeSent", parameter.ToArray()));

        return result;
    }

    public async Task<long> UpdateMessageAsync(Message message)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MessageID", message.MessageID));
        parameter.Add(new SqlParameter("@FromUserID", message.FromUserID));
        parameter.Add(new SqlParameter("@ToUserID", message.ToUserID));
        parameter.Add(new SqlParameter("@Value", message.Value));
        parameter.Add(new SqlParameter("@TimeSent", message.TimeSent));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UpdateMessage]
            @MessageID
            @FromUserID,
            @ToUserID,
            @Value,
            @TimeSent", parameter.ToArray()));

        return result;
    }

    public async Task<long> DeleteMessageAsync(long MessageID)
    {
        return await Task.Run(() => _context.Database.ExecuteSqlInterpolatedAsync
        ($"[Procedure_DeleteMessage] {MessageID}"));
    }


    #region Specialize Requests


    public async Task<long> GenerateMessageAsync(Message message, string username)
    {

      


        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@FromUserID", message.FromUserID));
        parameter.Add(new SqlParameter("@ToUserID", message.ToUserID));
        parameter.Add(new SqlParameter("@Value", message.Value));
        parameter.Add(new SqlParameter("@TimeSent", DateTime.Now));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_SendMessage]
            @FromUserID,
            @ToUserID,
            @Value,
            @TimeSent", parameter.ToArray()));

        return result;

    }
    public async Task<List<Message?>> ReadMessagesAsync(long userID)
    {
        
        var parameters = new SqlParameter[]
           {
            new SqlParameter("@UserID", userID),
            new SqlParameter("@TimeReceived", DateTime.Now)
           };

        var info = await Task.Run(() =>
        _context.Message
            .FromSqlRaw(@"EXECUTE [Procedure_GetMyMessages]
                    @UserID, @TimeReceived", parameters)
            .ToListAsync());


        return info;
    }



    #endregion
}
