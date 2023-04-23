using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Model;
using System.Data;

namespace RockPaperScissorsAPI.Services;

public class PlayerInviteService : IPlayerInviteService
{
    private readonly DataContext _context;
    public PlayerInviteService(DataContext context) => _context = context;

    public async Task<List<PlayerInvite>> GetAllPlayerInvitesAsync()
    {
        return await _context.PlayerInvite
        .FromSqlRaw("[Procedure_GetAllPlayerInvites]")
        .ToListAsync();
    }

    public async Task<IEnumerable<PlayerInvite>> GetPlayerInviteFromUserIDAsync(long InviteFromUserID)
    {
        var parameter = new SqlParameter("@FromUserID", InviteFromUserID);

        var info = await Task.Run(() =>
        _context.PlayerInvite
        .FromSqlRaw(@"EXECUTE [Procedure_GetPlayerInviteFromUserIDAsync] @FromUserID", parameter)
        .ToListAsync());

        return info;
    }

    public async Task<IEnumerable<PlayerInvite>> GetPlayerInviteToUserIDAsync(long InviteToUserID)
    {
        var parameter = new SqlParameter("@InviteToUserID", InviteToUserID);

        var info = await Task.Run(() =>
        _context.PlayerInvite
        .FromSqlRaw(@"EXECUTE [Procedure_GetPlayerInviteToUserIDAsync] @InviteToUserID", parameter)
        .ToListAsync());

        return info;
    }

    public async Task<long> CreatePlayerInvitationAsync(PlayerInvite playerInvite)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@InviteFromUserID", playerInvite.InviteFromUserID));
        parameter.Add(new SqlParameter("@InviteToUserID", playerInvite.InviteToUserID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerInvitation]
            @FromUserID,
            @ToUserID", parameter.ToArray()));

        return result;
    }
    public async Task<long> EditPlayerInvitationAsync(PlayerInvite playerInvite)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@InviteFromUserID", playerInvite.InviteFromUserID));
        parameter.Add(new SqlParameter("@InviteToUserID", playerInvite.InviteToUserID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_EditPlayerInvitation]
            @FromUserID,
            @ToUserID", parameter.ToArray()));

        return result;
    }
    public async Task<long> DeletePlayerInvitationAsync(long InviteFromUserID)
    {
        return await Task.Run(() => _context.Database.ExecuteSqlInterpolatedAsync
        ($"[Procedure_DeletePlayerInvitation] {InviteFromUserID}"));
    }

    #region Specialized Requests

    public async Task<long> InvitePlayerAsync(PlayerInvite playerInvite)
    {


        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@InviteFromUserID", playerInvite.InviteFromUserID));
        parameter.Add(new SqlParameter("@InviteToUserID", playerInvite.InviteToUserID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerInvitation]
            @InviteFromUserID,
            @InviteToUserID", parameter.ToArray()));

        return result;
    }

    public async Task<long> AcceptInviteAsync(PlayerInvite playerInvite)
    {


        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@InviteFromUserID", playerInvite.InviteFromUserID));
        parameter.Add(new SqlParameter("@InviteToUserID", playerInvite.InviteToUserID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CheckPlayerInviteBothID]
            @InviteFromUserID,
            @InviteToUserID", parameter.ToArray()));



        return result;
    }


    public async Task<PlayerInvite> FindInvitationAsync(long fromUserID, long toUserID)
    {
        var invite = await _context.PlayerInvite
            .Where(x => x.InviteFromUserID == fromUserID && x.InviteToUserID == toUserID)
            .SingleOrDefaultAsync();

        return invite;

    }


    #endregion

}
