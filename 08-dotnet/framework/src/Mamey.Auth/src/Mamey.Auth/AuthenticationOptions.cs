namespace Mamey.Auth;

/// <summary>
/// Unified authentication options for configuring Azure and JWT providers.
/// </summary>
public class AuthenticationOptions
{
    public AzureOptions Azure { get; set; } = new();
    public JwtOptions Jwt { get; set; } = new();
}