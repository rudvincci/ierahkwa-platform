using System.Text;

namespace Mamey.Identity.Jwt;

public class PasswordGenerator
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "!@#$%^&*";

    public static string GenerateTemporaryPassword(int length, bool includeSpecialChars)
    {
        var characterSet = LowerCase + UpperCase + Digits;
        if (includeSpecialChars)
        {
            characterSet += SpecialChars;
        }

        var password = new StringBuilder();
        var random = new Random();

        for (int i = 0; i < length; i++)
        {
            var index = random.Next(characterSet.Length);
            password.Append(characterSet[index]);
        }

        return password.ToString();
    }
}
