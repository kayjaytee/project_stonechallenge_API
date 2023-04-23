using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Validation;

public class UserScore
{
    [Key]
    public long UserID { get; set; }

    public long Wins { get; set; }
    public long Losses { get; set; }
    public long GamesPlayed { get; set; }

}
