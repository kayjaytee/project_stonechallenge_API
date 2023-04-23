using RockPaperScissorsAPI.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockPaperScissorsAPI.Model;

public class Message
{

    [Key]
    public long MessageID { get; set; }

    [ForeignKey("UserID")]
    public long FromUserID { get; set; }

    [ForeignKey("UserID")]
    public long ToUserID { get; set; }

    public string? Value { get; set; }

    public DateTime? TimeSent { get; set; }
    public DateTime? TimeReceived { get; set; }
}
