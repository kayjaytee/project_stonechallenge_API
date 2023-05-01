using RockPaperScissorsAPI.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockPaperScissorsAPI.Model;

public class Game
{

    [Key]
    public long GameID { get; set; }

    [ForeignKey("UserID")]
    public long PlayerOneID { get; set; }

    [ForeignKey("UserID")]
    public long PlayerTwoID { get; set; }

    [ForeignKey("MoveID")]
    public short? PlayerOneChoice { get; set; }

    [ForeignKey("MoveID")]
    public short? PlayerTwoChoice { get; set; }

    [ForeignKey("UserID")]
    public long? PlayerWinner { get; set; }

    [ForeignKey("MessageID")]
    public long? PlayerMessageID { get; set; }

    public bool IsFinished { get; set; }

}
