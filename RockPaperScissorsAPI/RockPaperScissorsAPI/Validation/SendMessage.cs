using Microsoft.EntityFrameworkCore;

namespace RockPaperScissorsAPI.Validation;


[Keyless]
public class SendMessage
{
    public string Value { get; set; } = string.Empty;

    public string ToUserName { get; set; } = string.Empty;

}
