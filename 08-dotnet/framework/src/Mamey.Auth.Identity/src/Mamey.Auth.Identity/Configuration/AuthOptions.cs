using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Mamey.Auth.Identity.Configuration;

public class AuthOptions : IdentityOptions
{
    public bool AuthenticationDisabled { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
    public CertificateOptions Certificate { get; set; } = new();
    public string IssuerSigningKey { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Challenge { get; set; } = "Bearer";
    public string MetadataAddress { get; set; } = string.Empty;
    public bool SaveToken { get; set; } = true;
    public bool SaveSigninToken { get; set; }
    public bool RequireAudience { get; set; } = true;
    public bool RequireHttpsMetadata { get; set; } = true;
    public bool RequireExpirationTime { get; set; } = true;
    public bool RequireSignedTokens { get; set; } = true;
    public string Algorithm { get; set; } = string.Empty;
    public TimeSpan? Expiry { get; set; }
    public TimeSpan? RefreshTokenLifetime { get; set; }
    public string ValidAudience { get; set; } = string.Empty;
    public IEnumerable<string> ValidAudiences { get; set; } = Enumerable.Empty<string>();
    public string ValidIssuer { get; set; } = string.Empty;
    public IEnumerable<string> ValidIssuers { get; set; } = Enumerable.Empty<string>();
    public bool ValidateActor { get; set; }
    public bool ValidateAudience { get; set; } = true;
    public bool ValidateIssuer { get; set; } = true;
    public bool ValidateLifetime { get; set; } = true;
    public bool ValidateTokenReplay { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public bool RefreshOnIssuerKeyNotFound { get; set; } = true;
    public bool IncludeErrorDetails { get; set; } = true;
    public string AuthenticationType { get; set; } = string.Empty;
    public string NameClaimType { get; set; } = string.Empty;
    public string RoleClaimType { get; set; } = string.Empty;
    public CookieOptions Cookie { get; set; }

    public record CookieOptions
    {
        public string Path { get; init; } = string.Empty;
        public bool HttpOnly { get; init; }
        public bool Secure { get; init; }
        public string SameSite { get; init; } = string.Empty;
        public string Domain { get; init; } = string.Empty;
    }
    public class CertificateOptions
    {
        public CertificateOptions()
        { }

        public string Location { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}