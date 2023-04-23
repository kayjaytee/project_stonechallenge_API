using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Model.ApiModels;
using RockPaperScissorsAPI.Validation;

namespace RockPaperScissorsAPI.Services;

public interface IUserService
{
    string GetMyName();
    string GetMyEmail();

    public Task<User> GetUserFromIDAsync(long userID);
    public Task<long> FindIdFromUsernameAsync(string username);
    public Task<string?> GetUsernameFromIdAsync(long userID);
    public Task<User> GetUserByUsernameOrEmailAsync(string username, string email);
    public Task<User> GetUsernameFromUserAsync(string username);

    public Task<List<User>> GetUsersAsync();
    public Task<IEnumerable<User>> GetUserByIdAsync(long UserID);
    public Task<long> CreateNewUserAsync(User user);
    public Task<long> UpdateUserAsync(User user);
    public Task<long> DeleteUserAsync(long UserID);

    #region Specialized Services

    public Task<long> RegisterUserAsync(CreateUserRequest user);
    public Task<IEnumerable<LoginUser>> LoginUserAsync(LoginUser request);
    public Task<long> GetEmailAsync(EmailUser request);
    public Task<long> ReplaceOldPassword(string username, string email, string password);
    public Task<long> InsertNewPassword(ResetPassword request);
    public Task<long> PostTokenAsync(User user);


    #endregion
}
