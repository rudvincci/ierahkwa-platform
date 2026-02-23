namespace AppBuilder.Core.Models;

/// <summary>JWT settings for Ierahkwa Appy. Issuer, audience, expiration.</summary>
public class JwtOptions
{
    public const string Section = "Jwt";
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "IerahkwaAppy";
    public string Audience { get; set; } = "IerahkwaAppy";
    public int ExpirationMinutes { get; set; } = 60; // 1 hour (was 7 days — OWASP A07)
}
