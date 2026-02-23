namespace Mamey.FWID.Identities.Infrastructure.Options;

/// <summary>
/// Options for email confirmation.
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Whether email confirmation is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Email confirmation token expiration time.
    /// </summary>
    public TimeSpan ConfirmationExpiration { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    /// Confirmation code length (for numeric codes).
    /// </summary>
    public int CodeLength { get; set; } = 6;

    /// <summary>
    /// Maximum number of confirmation requests per hour.
    /// </summary>
    public int MaxRequestsPerHour { get; set; } = 5;

    /// <summary>
    /// From email address for confirmation emails.
    /// </summary>
    public string FromEmail { get; set; } = "noreply@futurewampumid.com";

    /// <summary>
    /// From name for confirmation emails.
    /// </summary>
    public string FromName { get; set; } = "FutureWampumID";

    /// <summary>
    /// Email template name for confirmation emails.
    /// </summary>
    public string ConfirmationTemplate { get; set; } = "EmailConfirmation";

    /// <summary>
    /// Base URL for confirmation links.
    /// </summary>
    public string ConfirmationBaseUrl { get; set; } = "https://auth.futurewampumid.com";
}

