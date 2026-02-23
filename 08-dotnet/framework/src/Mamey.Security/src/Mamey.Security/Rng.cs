using System.Security.Cryptography;

namespace Mamey.Security;

public sealed class Rng : IRng
{
    private static readonly string[] SpecialChars = { "/", "\\", "=", "+", "?", ":", "&" };

    public string Generate(int length = 50, bool removeSpecialChars = true)
    {
        // Limit length to prevent OutOfMemoryException
        // Base64 encoding produces ~4/3 the size of input, so we need to limit
        const int maxLength = 100_000_000; // Reasonable maximum (100MB)
        if (length > maxLength)
        {
            throw new ArgumentException($"Length cannot exceed {maxLength}. Requested: {length}", nameof(length));
        }
        
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        var result = Convert.ToBase64String(bytes);

        return removeSpecialChars
            ? SpecialChars.Aggregate(result, (current, chars) => current.Replace(chars, string.Empty))
            : result;
    }
}