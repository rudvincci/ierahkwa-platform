namespace Mamey.FWID.Identities.Infrastructure.Options;

/// <summary>
/// Options for SMS confirmation.
/// </summary>
public class SmsOptions
{
    /// <summary>
    /// Whether SMS confirmation is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// SMS confirmation code expiration time.
    /// </summary>
    public TimeSpan ConfirmationExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Confirmation code length (numeric).
    /// </summary>
    public int CodeLength { get; set; } = 6;

    /// <summary>
    /// Maximum number of confirmation requests per hour.
    /// </summary>
    public int MaxRequestsPerHour { get; set; } = 5;

    /// <summary>
    /// SMS provider (e.g., "Twilio", "AWS", "Azure").
    /// </summary>
    public string Provider { get; set; } = "Twilio";

    /// <summary>
    /// From phone number for SMS (Twilio format: +1234567890).
    /// </summary>
    public string? FromPhoneNumber { get; set; }

    /// <summary>
    /// SMS message template. Use {Code} placeholder for the confirmation code.
    /// </summary>
    public string MessageTemplate { get; set; } = "Your FutureWampum verification code is: {Code}. This code expires in {ExpirationMinutes} minutes.";
}

