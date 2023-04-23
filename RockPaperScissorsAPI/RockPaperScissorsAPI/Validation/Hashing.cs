using System.Security.Cryptography;
using System.Text;
using BCrypt;

namespace RockPaperScissorsAPI.Validation;

public class Hashing
{
    public static string GenerateRandomPassword(int length)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var rng = new RNGCryptoServiceProvider();
        byte[] bytes = new byte[length];
        rng.GetBytes(bytes);
        var chars = bytes.Select(b => validChars[b % validChars.Length]);
        return new string(chars.ToArray());
    }
}

