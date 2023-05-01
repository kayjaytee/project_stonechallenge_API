using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RockPaperScissorsAPI.Data;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Model.ApiModels;
using RockPaperScissorsAPI.Validation;
using System.Security.Claims;
using BCrypt.Net;
using Azure.Core;

namespace RockPaperScissorsAPI.Services;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(DataContext context, IHttpContextAccessor httpContextAccessor)
    { 
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    #region HttpContext
    public string GetMyName()
    {
        var result = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            }
            return result;
    }
    
    public string GetMyEmail()
    {
        var result = string.Empty;
        if (_httpContextAccessor.HttpContext != null)
        {
            result = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }
        return result;
    }
    #endregion

    public async Task<User> GetUserFromIDAsync(long userID)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.UserID == userID);
    }

    public async Task<long> FindIdFromUsernameAsync(string username)
    {
        var getUserID = await _context.User.FirstOrDefaultAsync(u => u.UserName == username);
        return getUserID.UserID;
    }

    public async Task<string?> GetUsernameFromIdAsync(long userID)
    {
        var getUsername = await _context.User.FirstOrDefaultAsync(u => u.UserID == userID);
        return getUsername?.UserName;
    }

    public async Task<User> GetUserByUsernameOrEmailAsync(string username, string email)
    {
        return await _context.User.FirstOrDefaultAsync(u => u.UserName == username || u.UserEmail == email);
    }

    public async Task<User> GetUsernameFromUserAsync(string username) =>
        await _context.User.FirstOrDefaultAsync(u => u.UserName == username);

    public async Task<List<User>> GetUsersAsync()
    {
        return await _context.User
        .FromSqlRaw("[Procedure_GetUsers]")
        .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUserByIdAsync(long UserID)
    {
        var parameter = new SqlParameter("@UserID", UserID);

        var info = await Task.Run(() =>
        _context.User
        .FromSqlRaw(@"EXECUTE [Procedure_GetUserByID] @UserID", parameter)
        .ToListAsync());

        return info;
    }

    public async Task<long> CreateNewUserAsync(User user)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@UserName", user.UserName));
        parameter.Add(new SqlParameter("@FirstName", user.FirstName));
        parameter.Add(new SqlParameter("@LastName", user.LastName));
        parameter.Add(new SqlParameter("@UserEmail", user.UserEmail));
        parameter.Add(new SqlParameter("@Password", passwordHash));
        parameter.Add(new SqlParameter("@Token", "N/A"));
        parameter.Add(new SqlParameter("@TokenIssued", DateTime.Now));
        parameter.Add(new SqlParameter("@Wins", user.Wins));
        parameter.Add(new SqlParameter("@Losses", user.Losses));
        parameter.Add(new SqlParameter("@GamesPlayed", user.GamesPlayed));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_CreateNewUser]
            @UserName,
            @FirstName,
            @LastName,
            @UserEmail,
            @Password,
            @Token,
            @TokenIssued,
            @Wins,
            @Losses,
            @GamesPlayed", parameter.ToArray()));

        return result;
    }

    public async Task<long> UpdateUserAsync(User user)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@UserID", user.UserID));
        parameter.Add(new SqlParameter("@UserName", user.UserName));
        parameter.Add(new SqlParameter("@FirstName", user.FirstName));
        parameter.Add(new SqlParameter("@LastName", user.LastName));
        parameter.Add(new SqlParameter("@UserEmail", user.UserEmail));
        parameter.Add(new SqlParameter("@Password", passwordHash));
        parameter.Add(new SqlParameter("@Token", "N/A"));
        parameter.Add(new SqlParameter("@TokenIssued", DateTime.Now));
        parameter.Add(new SqlParameter("@Wins", user.Wins));
        parameter.Add(new SqlParameter("@Losses", user.Losses));
        parameter.Add(new SqlParameter("@GamesPlayed", user.GamesPlayed));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UpdateUser]
            @UserID,
            @UserName,
            @FirstName,
            @LastName,
            @UserEmail,
            @Password,
            @Token,
            @TokenIssued,
            @Wins,
            @Losses,
            @GamesPlayed", parameter.ToArray()));

        return result;
    }

    public async Task<long> DeleteUserAsync(long UserID)
    {
        return await Task.Run(() => _context.Database.ExecuteSqlInterpolatedAsync
        ($"[Procedure_DeleteUser] {UserID}"));
    }

    #region Specialized Requests
    public async Task<long> RegisterUserAsync(CreateUserRequest user)
    {
        int newaccount = 0;
        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@UserName", user.UserName));
        parameter.Add(new SqlParameter("@FirstName", user.FirstName));
        parameter.Add(new SqlParameter("@LastName", user.LastName));
        parameter.Add(new SqlParameter("@UserEmail", user.UserEmail));
        parameter.Add(new SqlParameter("@Password", passwordHash));
        parameter.Add(new SqlParameter("@Token", "N/A"));
        parameter.Add(new SqlParameter("@TokenIssued", DateTime.Now));
        parameter.Add(new SqlParameter("@Wins", newaccount));
        parameter.Add(new SqlParameter("@Losses", newaccount));
        parameter.Add(new SqlParameter("@GamesPlayed", newaccount));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UserAdd]
            @UserName,
            @FirstName,
            @LastName,
            @UserEmail,
            @Password,
            @Token,
            @TokenIssued,
            @Wins,
            @Losses,
            @GamesPlayed", parameter.ToArray()));

        return result;
    }

    public async Task<IEnumerable<LoginUser>> LoginUserAsync(LoginUser request)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@UserName", request.UserName));

        var result = await Task.Run(() =>
        _context.LoginUser
        .FromSqlRaw(@"EXECUTE [Procedure_AttemptLogin] @UserName, @Password", parameter)
        .ToListAsync());

        return result;
    }

    #region Email/RestorePasswordService
    public async Task<long> GetEmailAsync(EmailUser request)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@UserEmail", request.UserEmail));
        parameter.Add(new SqlParameter("@UserName", request.UserName));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_GetEmail]
            @UserEmail,
            @UserName", parameter.ToArray()));

        return result;

    }
    public async Task<long> ReplaceOldPassword(string username, string email, string password)
    {

        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@ReplaceOldPassword", password));
        parameter.Add(new SqlParameter("@UserName", username));
        parameter.Add(new SqlParameter("@UserEmail", email));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_ForgotPasswordNewGenerated]
            @ReplaceOldPassword,
            @UserName,
            @UserEmail", parameter.ToArray()));

        return result;
    }

    public async Task<long> InsertNewPassword(ResetPassword request)
    {
        var parameter = new List<SqlParameter>();
        parameter.Add(new SqlParameter("@NewPassword", request.NewPassword));
        parameter.Add(new SqlParameter("@OldPassword", request.OldPassword));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_UpdateWithNewPassword]
            @NewPassword,
            @OldPassword", parameter.ToArray()));

        return result;
    }
    #endregion
    public async Task<long> PostTokenAsync(User user)
    {
        var parameter = new List<SqlParameter>();

        parameter.Add(new SqlParameter("@Token", user.Token));
        parameter.Add(new SqlParameter("@TokenIssued", DateTime.Now));
        parameter.Add(new SqlParameter("@UserName", user.UserName));

        var result = await Task.Run(() =>
        _context.Database.ExecuteSqlRawAsync
        (@"EXECUTE [Procedure_PostToken]
            @Token,
            @TokenIssued,
            @UserName", parameter.ToArray()));

        return result;
    }

    //public async Task<long> TokenCompare(User user)
    //{
    //    var tokenCompare = _context.User.FirstOrDefaultAsync(x => x.Token == user.Token);
    //    if (tokenCompare != null)
    //    {

    //    }
    //    var tokenTime = _context.User.FirstOrDefaultAsync(x => x.TokenIssued == user.TokenIssued);

    //}

    #endregion
}
