using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Validation;

[Keyless]
public class LoginUser
{

    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
