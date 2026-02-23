using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MameyNode.UI.Services;

/// <summary>
/// Minimal HS256 JWT generator (no external packages).
/// Designed for dev-mode SDK auth; aligns with MameyNode's default JWT secret when configured.
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateJwt(string subject, IReadOnlyList<string> permissions, TimeSpan ttl)
    {
        var secret = _configuration["MameyNode:Authentication:JwtSecret"]
            ?? "dev-jwt-secret-key-change-in-production";

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + (long)ttl.TotalSeconds;

        var headerJson = JsonSerializer.Serialize(new { alg = "HS256", typ = "JWT" });
        var payloadJson = JsonSerializer.Serialize(new
        {
            sub = subject,
            iat = now,
            exp,
            permissions = permissions
        });

        var header = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var signingInput = $"{header}.{payload}";

        var signature = HmacSha256(signingInput, secret);
        return $"{signingInput}.{Base64UrlEncode(signature)}";
    }

    private static byte[] HmacSha256(string data, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

