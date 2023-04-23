using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Validation;

[Keyless]
public class ResetPassword
{


    [Required(ErrorMessage = "")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "")]
    public string NewPassword { get; set; }



}
