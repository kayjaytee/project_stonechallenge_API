using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Services;

public interface IGameService
{
    public Task<List<Game>> GetGamesAsync();
    public Task<IEnumerable<Game>> GetGameByIdAsync(long GameID);
    public Task<long> CreateNewGameAsync(Game game);
    public Task<long> UpdateGameAsync(Game game);
    public Task<long> DeleteGameAsync(long GameID);


    #region Specialized Requests

    public Task<long> HostNewGameAsync(Game game, PlayerInvite playerInvite);
    public Task<long> PlayerOneMove(Game game, PlayerMoves playerMoves);
    public Task<long> PlayerTwoMove(Game game, PlayerMoves playerMoves);
    public Task<long> DetermineWinnerAsync(Game game);
    public Task<long> IncreaseGamesPlayedAsync(Game game, UserScore user);
    public Task<Game> FetchGameIDAsync(long gameID);

    #endregion
}
