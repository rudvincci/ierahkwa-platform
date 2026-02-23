using System.Security.Cryptography;

namespace Mamey.Utilities;

public static class Utility
{
    public static string GetRandomHexColor()
        => $"#{new Random().Next(0x1000000):X6}";

    
    public static string GenerateRandomNumber(int length = 50, bool removeSpecialChars = true)
    {
        string[] SpecialChars = new[] { "/", "\\", "=", "+", "?", ":", "&" };

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var result = Convert.ToBase64String(bytes);

        return removeSpecialChars
            ? SpecialChars.Aggregate(result, (current, chars) => current.Replace(chars, string.Empty))
            : result;
    }
}

