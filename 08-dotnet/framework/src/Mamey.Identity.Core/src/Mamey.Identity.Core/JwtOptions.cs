namespace Mamey.Identity.Core;

/// <summary>
/// JWT-specific configuration options.
/// </summary>
public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
