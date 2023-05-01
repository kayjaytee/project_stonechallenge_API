using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Services;

public class PlayerMovesService : IPlayerMovesService
{
    private readonly DataContext _context;
    public PlayerMovesService(DataContext context) => _context = context;

    public async Task<List<PlayerMoves>> GetPlayerMovesAsync()
    {
        return await _context.PlayerMoves
        .FromSqlRaw("[Procedure_GetPlayerMoves]")
        .ToListAsync();
    }

    public async Task<IEnumerable<PlayerMoves>> GetPlayerMovesByIDAsync(byte MoveID)
    {
        var parameter = new SqlParameter("@MoveID", MoveID);
        var info = await Task.Run(() =>
        _context.PlayerMoves
        .FromSqlRaw(@"EXECUTE [Procedure_GetPlayerMovesByID] @MoveID", parameter)
        .ToListAsync());

        return info;
    }

    public async Task<long> CreatePlayerMoveAsync(PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MovesTitle", playerMoves.MovesTitle));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerMove]
            @MovesTitle", parameter.ToArray()));

        return result;
    }

    public async Task<long> UpdatePlayerMoveAsync(PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MoveID", playerMoves.MoveID));
        parameter.Add(new SqlParameter("@MovesTitle", playerMoves.MovesTitle));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerMove]
            @MoveID,
            @MovesTitle", parameter.ToArray()));

        return result;
    }

    public async Task<long> DeletePlayerMoveAsync(byte MoveID)
    {
        return await Task.Run(() => _context.Database.ExecuteSqlInterpolatedAsync
        ($"[Procedure_DeletePlayerMove] {MoveID}"));
    }



    #region Specialized Requests


    //MoveID = 1
    public async Task<long> RockAsync(PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MovesTitle", playerMoves.MovesTitle));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerMove]
            @MovesTitle", parameter.ToArray()));

        return result;
    }

    //MoveID = 2
    public async Task<long> PaperAsync(PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MovesTitle", playerMoves.MovesTitle));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerMove]
            @MovesTitle", parameter.ToArray()));

        return result;
    }

    //MoveID = 3
    public async Task<long> ScissorAsync(PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@MovesTitle", playerMoves.MovesTitle));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreatePlayerMove]
            @MovesTitle", parameter.ToArray()));

        return result;
    }


    #endregion
}
