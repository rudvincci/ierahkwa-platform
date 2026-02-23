namespace Mamey.Identity.Jwt;

public class JwtOptions
{
    public TokenStorageType TokenStorage { get; set; } = TokenStorageType.Memory;
    public bool AuthenticationDisabled { get; set; }
    public IEnumerable<string> AllowAnonymousEndpoints { get; set; } = Enumerable.Empty<string>();
    public CertificateOptions Certificate { get; set; } = new();
    public string Algorithm { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
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
    public int ExpiryMinutes { get; set; }
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
    public CookieOptions Cookie { get; set; } = new();

    public class CertificateOptions
    {
        public CertificateOptions()
        { }

        public string Location { get; set; } = string.Empty;
        public string RawData { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class CookieOptions
    {
        public bool HttpOnly { get; set; }
        public bool Secure { get; set; }
        public string SameSite { get; set; }
    }
}

public enum TokenStorageType
{
    Memory,
    Redis
}
