using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Validation;

[Keyless]
public class EmailUser
{
    [Required, EmailAddress, Display(Name = "Registered E-Mail Adress")]
    public string UserEmail { get; set; }

    [Required(ErrorMessage = "You need to fill in your username")]
    public string UserName { get; set; }

    //[Required(ErrorMessage = "")]
    //public string Token { get; set; }
}
