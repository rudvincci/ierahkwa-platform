using System.Security.Cryptography;
using System.Text;
using Mamey.MicroMonolith.Infrastructure.Security.Encryption;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal class TokenService : ITokenService
{
    private readonly IRng _rng;

    public TokenService(IRng rng)
    {
        _rng = rng;
    }

    public async Task<string> GenerateTokenAsync(CancellationToken cancellationToken = default)
    {
        // Generate a secure random token using IRng
        // Generate 32 bytes (256 bits) for a strong token
        // IRng.Generate returns a Base64 string, convert to URL-safe format
        var randomBase64 = _rng.Generate(32, removeSpecialChars: false);
        
        // Convert to Base64 URL-safe string (removes padding and replaces +/ with -_)
        var token = randomBase64
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
        
        return await Task.FromResult(token);
    }

    public string HashToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        // Compute SHA256 hash
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        
        // Convert to hexadecimal string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public bool ValidateToken(string token, string hash)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(hash))
            return false;

        var computedHash = HashToken(token);
        return string.Equals(computedHash, hash, StringComparison.OrdinalIgnoreCase);
    }
}
