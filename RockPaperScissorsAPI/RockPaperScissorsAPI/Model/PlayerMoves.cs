using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Model;

public class PlayerMoves
{
    [Key]
    public short MoveID { get; set; }

    public string MovesTitle { get; set; }
}
