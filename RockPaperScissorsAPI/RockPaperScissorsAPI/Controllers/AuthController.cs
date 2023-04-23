using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RockPaperScissorsAPI.Model;
using RockPaperScissorsAPI.Model.ApiModels;
using RockPaperScissorsAPI.Services;
using RockPaperScissorsAPI.Validation;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace RockPaperScissorsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]

public class AuthController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IConfiguration configuration;
    private readonly IHttpContextAccessor httpContextAccessor;
    internal static User user = new User();

    public AuthController(IUserService userService,
                          IConfiguration configuration,
                          IHttpContextAccessor httpContextAccessor)
    {
        this.userService = userService;
        this.configuration = configuration;
        this.httpContextAccessor = httpContextAccessor;
    }




    // a. Skapa användare (Sparas till databasen)
    [HttpPost("RegisterNewUser")]
    public async Task<ActionResult> RegisterUserAsync(CreateUserRequest user)
    {

        if (user == null)
        {
            return BadRequest();
        }

        try
        {
            var existingUser = await userService.GetUserByUsernameOrEmailAsync(user.UserName, user.UserEmail);
            if (existingUser != null)
            {
                if (existingUser.UserName == user.UserName)
                {
                    return BadRequest("Username is already taken");
                }
                if (existingUser.UserEmail == user.UserEmail)
                {
                    return BadRequest("Email is already taken.");
                }
            }

            var response = await userService.RegisterUserAsync(user);

            return Ok("A new user has been created!");


        }
        catch
        {
            throw;
        }
    }


    // b. Logga in användare (Läser från databasen)
    [HttpPost("Login")]
    public async Task<ActionResult<string>> AttemptLogin(LoginUser request)
    {
        try
        {
            var findUser = await userService.GetUsernameFromUserAsync(request.UserName);
            if (findUser == null)
            {
                return BadRequest("Invalid username or password");
            }

            var passwordHash = BCrypt.Net.BCrypt.Verify(request.Password, findUser.Password);

            if (!passwordHash)
            {
                return BadRequest("Invalid username or password");
            }

            //Token
            user.UserName = request.UserName;
            var userID = await userService.FindIdFromUsernameAsync(request.UserName);
            user.UserID = (long)userID;
            string token = GenerateLoginToken(user);
            user.Token = token;
            user.TokenIssued = DateTime.Now.AddMinutes(10);
            var tokenresponse = await userService.PostTokenAsync(user);

            return Ok(token);


        }
        catch
        {
            throw;
        }
    }

    [HttpPost("LogOut"), Authorize(Policy = "Authenticated for Login")]
    public async Task<ActionResult<User>> LogOut()
    {

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Ok();
    }

    [HttpGet("AuthenticateLogin"), Authorize(Policy = "Authenticated for Login")]
    public async Task<ActionResult<string>> GetMeAsync()
    {
        var userName = userService.GetMyName();
        return Ok("Welcome " + userName + "!");

    }


    #region g. Lösenordsåterställning; i.Utskick av återställningskod till användarens e-postadress.

    [HttpPost("bearer/ForgotPassword")]
    [AllowAnonymous]
    public async Task<ActionResult> ForgotPassword(EmailUser request)
    {

        user.UserEmail = request.UserEmail;
        user.UserName = request.UserName;

        try
        {
            var getmailresponse = await userService.GetEmailAsync(request);

            if (getmailresponse == null)
            {
                return BadRequest("Invalid request");
            }

            //var sendmailresponse = SendEmail(request); -- THIS IS TURNED OFF BECAUSE OF SMTP CLIENT ISSUES, READ MORE IN THE FUNCTION SUMMARY

            string token = GeneratePasswordResetTokenAsync(user);
            user.Token = token;
            return Ok(token);
        }
        catch
        {

            throw;
        }




    }

    [HttpPost("email/SendEmail")]
    public ActionResult SendEmail(EmailUser request)
    {

        /// <summary>
        /// I Had trouble setting up a gmail account as host/client for sending mail
        ///  due to updated security and too many authetications to go through and I dont want to use a personal mail for testing.
        ///  This is done using ethereal.email, which is a mail service for testing.
        ///  Replace the "var from" mailadress with a new one and update credentials with new password.
        /// 
        /// 
        ///  READ MORE HERE: https://ethereal.email/
        /// </summary>

        try
        {



            string generatedpassword = Hashing.GenerateRandomPassword(12);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedpassword, 12);
            string subject = "RockPaperScissors - Generate a New Password";
            string body = "Here's a new password to authenticate your account with: \n\n " + generatedpassword;

            var from = new MailAddress("gwendolyn73@ethereal.email", "gwendolyn73@ethereal.email");
            var to = new MailAddress("gwendolyn73@ethereal.email", request.UserName);


            var smtp = new SmtpClient
            {
                Host = "smtp.ethereal.email",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(from.Address, "psnUGh98kEvu5vt56U"),
                Timeout = 20000

            };
            using (var message = new MailMessage(from, to)
            {
                Body = body,
                Subject = subject
            })
            {
                smtp.Send(message);
            }
            return Ok("Email Sent to address: " + request.UserEmail);
        }
        catch
        {
            return BadRequest("Input Error.");
        }
    }

    [HttpPut("bearer/GenerateNewPassword")]
    [Authorize(Policy = "Authentication for Generating a New Password")]
    public async Task<ActionResult<string>> GenerateNewPassword()
    {

        var userName = userService.GetMyName();
        var email = userService.GetMyEmail();

        user.UserName = userName;
        user.UserEmail = email;

        string generatedpassword = Hashing.GenerateRandomPassword(12);
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(generatedpassword, salt);

        var passwordToDB = await userService.ReplaceOldPassword(user.UserName, user.UserEmail, hashedPassword);
        var tokenresponse = await userService.PostTokenAsync(user);

        return Ok("This is your temporary password; use this to create a new password: " + generatedpassword);
    }

    [HttpPut("bearer/CreateTheNewPassword")]
    [Authorize(Policy = "Authentication for Generating a New Password")]
    public async Task<ActionResult<string>> CreateTheNewPassword(ResetPassword request)
    {
        try
        {
            var userName = userService.GetMyName();
            var email = userService.GetMyEmail();

            var findUser = await userService.GetUserByUsernameOrEmailAsync(userName, email);
            if (findUser == null)
            {
                return BadRequest("Invalid password");
            }
            
            string passwordFromDB = findUser.Password;
            var existingpasswordHash = BCrypt.Net.BCrypt.Verify(request.OldPassword, passwordFromDB);
            if (!existingpasswordHash)
            {
                return BadRequest("Invalid password");
            }

            request.OldPassword = findUser.Password;

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            var newpasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, salt);

            request.NewPassword = newpasswordHash;


            var passwordToDB = await userService.InsertNewPassword(request);

   

            return Ok("You have created a new password!");
        }
        catch
        {
            return BadRequest("Invalid password");
        }
    }

    #endregion

    #region Tokens
    private string GenerateLoginToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Authentication, "Token"),
            new Claim(ClaimTypes.AuthenticationMethod, "Authenticated for Login"),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: creds
            );



        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private string GeneratePasswordResetTokenAsync(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Authentication, "Token"),
            new Claim(ClaimTypes.AuthenticationMethod, "Authentication for Generating a New Password"),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Email, user.UserEmail),
            new Claim(ClaimTypes.Name, user.UserName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),
            signingCredentials: creds
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }


    #endregion




}
