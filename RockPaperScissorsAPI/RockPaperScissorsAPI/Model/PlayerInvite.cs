using System.ComponentModel.DataAnnotations.Schema;

namespace RockPaperScissorsAPI.Model;

public class PlayerInvite
{

    [ForeignKey("UserID")]
    public long InviteFromUserID { get; set; }

    [ForeignKey("UserID")]
    public long InviteToUserID { get; set; }

}
