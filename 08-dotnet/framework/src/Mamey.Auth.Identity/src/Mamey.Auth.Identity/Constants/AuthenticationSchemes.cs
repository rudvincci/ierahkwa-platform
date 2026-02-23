namespace Mamey.Auth.Identity.Constants;

/// <summary>
/// Names of authentication schemes registered in DI.
/// </summary>
public static class AuthenticationSchemes
{
    public const string JwtBearer       = "Bearer";
    public const string Certificate     = "Certificate";
    public const string Cookie          = "Cookies";
    public const string ExternalOAuth   = "External";
}