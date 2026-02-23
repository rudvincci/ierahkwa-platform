namespace Mamey.FWID.Identities.Infrastructure.Options;

/// <summary>
/// Options for multi-factor authentication.
/// </summary>
public class MfaOptions
{
    /// <summary>
    /// Whether MFA is enabled globally.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Default MFA method to use when setting up MFA.
    /// </summary>
    public string DefaultMethod { get; set; } = "TOTP";

    /// <summary>
    /// TOTP issuer name (shown in authenticator apps).
    /// </summary>
    public string TotpIssuer { get; set; } = "FutureWampumID";

    /// <summary>
    /// TOTP account name format (e.g., "{Email}" or "{Username}").
    /// </summary>
    public string TotpAccountNameFormat { get; set; } = "{Email}";

    /// <summary>
    /// Number of backup codes to generate.
    /// </summary>
    public int BackupCodeCount { get; set; } = 10;

    /// <summary>
    /// MFA challenge expiration time.
    /// </summary>
    public TimeSpan ChallengeExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Maximum number of failed MFA attempts before locking.
    /// </summary>
    public int MaxFailedAttempts { get; set; } = 5;

    /// <summary>
    /// Lockout duration after max failed attempts.
    /// </summary>
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}

