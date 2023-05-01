using RockPaperScissorsAPI.Model;

namespace RockPaperScissorsAPI.Services;

public interface IPlayerMovesService
{
    public Task<List<PlayerMoves>> GetPlayerMovesAsync();
    public Task<IEnumerable<PlayerMoves>> GetPlayerMovesByIDAsync(byte MoveID);
    public Task<long> CreatePlayerMoveAsync(PlayerMoves playerMoves);
    public Task<long> UpdatePlayerMoveAsync(PlayerMoves playerMoves);
    public Task<long> DeletePlayerMoveAsync(byte MoveID);
}
