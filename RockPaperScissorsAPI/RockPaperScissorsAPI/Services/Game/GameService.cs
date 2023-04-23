using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Services;

public class GameService : IGameService
{
    private readonly DataContext _context;
    public GameService(DataContext context) => _context = context;


    public async Task<List<Game>> GetGamesAsync()
    {
        return await _context.Game
        .FromSqlRaw("[Procedure_GetGames]")
        .ToListAsync();
    }
    public async Task<IEnumerable<Game>> GetGameByIdAsync(long GameID)
    {
        var parameter = new SqlParameter("@GameID", GameID);

        var info = await Task.Run(() =>
        _context.Game
        .FromSqlRaw(@"EXECUTE [Procedure_GetGameByID] @GameID", parameter)
        .ToListAsync());

        return info;
    }
    public async Task<long> CreateNewGameAsync(Game game)
    {

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@PlayerOneID", game.PlayerOneID));
        parameter.Add(new SqlParameter("@PlayerTwoID", game.PlayerTwoID));
        parameter.Add(new SqlParameter("@PlayerOneChoice", game.PlayerOneChoice));
        parameter.Add(new SqlParameter("@PlayerTwoChoice", game.PlayerTwoChoice));
        parameter.Add(new SqlParameter("@PlayerWinner", game.PlayerWinner));
        parameter.Add(new SqlParameter("@PlayerMessageID", game.PlayerMessageID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreateNewGame]
            @PlayerOneID,
            @PlayerTwoID,
            @PlayerOneChoice,
            @PlayerTwoChoice,
            @PlayerWinner,
            @PlayerMessageID", parameter.ToArray()));

        return result;
    }
    public async Task<long> UpdateGameAsync(Game game)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@PlayerOneID", game.PlayerOneID));
        parameter.Add(new SqlParameter("@PlayerTwoID", game.PlayerTwoID));
        parameter.Add(new SqlParameter("@PlayerOneChoice", game.PlayerOneChoice));
        parameter.Add(new SqlParameter("@PlayerTwoChoice", game.PlayerTwoChoice));
        parameter.Add(new SqlParameter("@PlayerWinner", game.PlayerWinner));
        parameter.Add(new SqlParameter("@PlayerMessageID", game.PlayerMessageID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UpdateGame]
            @PlayerOneID,
            @PlayerTwoID,
            @PlayerOneChoice,
            @PlayerTwoChoice,
            @PlayerWinner,
            @PlayerMessageID", parameter.ToArray()));

        return result;

    }
    public async Task<long> DeleteGameAsync(long GameID)
    {
        return await Task.Run(() => _context.Database.ExecuteSqlInterpolatedAsync
        ($"[Procedure_DeleteGame] {GameID}"));

    }

    #region Specialized Requests

    public async Task<long> HostNewGameAsync(Game game, PlayerInvite playerInvite)
    {

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@PlayerOneID", playerInvite.InviteFromUserID));
        parameter.Add(new SqlParameter("@PlayerTwoID", playerInvite.InviteToUserID));
        parameter.Add(new SqlParameter("@PlayerOneChoice", (object)game.PlayerOneChoice ?? DBNull.Value));
        parameter.Add(new SqlParameter("@PlayerTwoChoice", (object)game.PlayerTwoChoice ?? DBNull.Value));
        parameter.Add(new SqlParameter("@PlayerWinner", (object)game.PlayerWinner ?? DBNull.Value));
        parameter.Add(new SqlParameter("@PlayerMessageID", (object)game.PlayerMessageID ?? DBNull.Value));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreateNewGame]
            @PlayerOneID,
            @PlayerTwoID,
            @PlayerOneChoice,
            @PlayerTwoChoice,
            @PlayerWinner,
            @PlayerMessageID", parameter.ToArray()));

        return result;
    }

    public async Task<long> PlayerOneMove(Game game, PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@PlayerOneID", game.PlayerOneID));
        parameter.Add(new SqlParameter("@PlayerOneChoice", playerMoves.MoveID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_PlayerOneMove]
            @PlayerOneID,
            @PlayerOneChoice", parameter.ToArray()));

        return result;

    }

    public async Task<long> PlayerTwoMove(Game game, PlayerMoves playerMoves)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@PlayerTwoID", game.PlayerTwoID));
        parameter.Add(new SqlParameter("@PlayerTwoChoice", playerMoves.MoveID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_PlayerTwoMove]
            @PlayerTwoID,
            @PlayerTwoChoice", parameter.ToArray()));

        return result;

    }

    public async Task<long> DetermineWinnerAsync(Game game)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@GameID", game.GameID));
        parameter.Add(new SqlParameter("@PlayerWinner", game.PlayerWinner));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_DetermineWinner]
            @GameID,
            @PlayerWinner", parameter.ToArray()));

        return result;

    }

    public async Task<long> IncreaseGamesPlayedAsync(Game game, UserScore user)
    {

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@GameID", game.GameID));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UpdateScore]
            @GameID", parameter.ToArray()));

        return result;

    }

    public async Task<Game> FetchGameIDAsync(long gameID)
    {
        return await _context.Game.FirstOrDefaultAsync(x => x.GameID == gameID);
    }

    #endregion

}
