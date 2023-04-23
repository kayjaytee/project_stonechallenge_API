using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Model;

public class User
{

    [Key]
    public long UserID { get; set; }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserEmail { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }

    public DateTime? TokenIssued { get; set; }

    public long Wins { get; set; }

    public long Losses { get; set; }

    public long GamesPlayed { get; set; }
}
