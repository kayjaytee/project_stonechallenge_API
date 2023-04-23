using RockPaperScissorsAPI.Model;
using System.Security.Claims;

namespace RockPaperScissorsAPI.Services;

public interface IPlayerInviteService
{
    public Task<List<PlayerInvite>> GetAllPlayerInvitesAsync();
    public Task<IEnumerable<PlayerInvite>> GetPlayerInviteFromUserIDAsync(long InviteFromUserID);
    public Task<IEnumerable<PlayerInvite>> GetPlayerInviteToUserIDAsync(long InviteToUserID);
    public Task<long> CreatePlayerInvitationAsync(PlayerInvite playerInvite);
    public Task<long> EditPlayerInvitationAsync(PlayerInvite playerInvite);
    public Task<long> DeletePlayerInvitationAsync(long InviteFromUserID);

    #region Specialized Requests

    public Task<long> InvitePlayerAsync(PlayerInvite playerInvite);

    public Task<long> AcceptInviteAsync(PlayerInvite playerInvite);

    public Task<PlayerInvite> FindInvitationAsync(long fromUserID, long toUserID);

    #endregion
}
