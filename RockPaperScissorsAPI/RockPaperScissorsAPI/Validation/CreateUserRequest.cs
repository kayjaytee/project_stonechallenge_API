using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RockPaperScissorsAPI.Model.ApiModels;

[Keyless]
public class CreateUserRequest
{

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    [Required]
    [EmailAddress] public string UserEmail { get; set; }

    [Required]
    public string Password { get; set; }

}
