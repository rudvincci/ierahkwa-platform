using System.ComponentModel;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class MfaChallenge : AggregateRoot<MfaChallengeId>
{
    public MfaChallenge(MfaChallengeId id, MultiFactorAuthId multiFactorAuthId, MfaMethod method,
        string challengeData, DateTime createdAt, DateTime expiresAt, MfaChallengeStatus status = MfaChallengeStatus.Pending,
        DateTime? verifiedAt = null, string? ipAddress = null, string? userAgent = null, int version = 0)
        : base(id, version)
    {
        MultiFactorAuthId = multiFactorAuthId;
        Method = method;
        ChallengeData = challengeData;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        Status = status;
        VerifiedAt = verifiedAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    #region Properties

    /// <summary>
    /// Reference to the MFA this challenge belongs to.
    /// </summary>
    [Description("Reference to the MFA this challenge belongs to")]
    public MultiFactorAuthId MultiFactorAuthId { get; private set; }

    /// <summary>
    /// MFA method used for this challenge.
    /// </summary>
    [Description("MFA method used for this challenge")]
    public MfaMethod Method { get; private set; }

    /// <summary>
    /// Challenge data (e.g., code, token, etc.).
    /// </summary>
    [Description("Challenge data")]
    public string ChallengeData { get; private set; }

    /// <summary>
    /// Date and time the challenge was created.
    /// </summary>
    [Description("Date and time the challenge was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the challenge expires.
    /// </summary>
    [Description("Date and time the challenge expires")]
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// Current status of the challenge.
    /// </summary>
    [Description("Current status of the challenge")]
    public MfaChallengeStatus Status { get; private set; }

    /// <summary>
    /// Date and time the challenge was verified.
    /// </summary>
    [Description("Date and time the challenge was verified")]
    public DateTime? VerifiedAt { get; private set; }

    /// <summary>
    /// Number of verification attempts.
    /// </summary>
    [Description("Number of verification attempts")]
    public int AttemptCount { get; private set; }

    /// <summary>
    /// IP address from which the challenge was created.
    /// </summary>
    [Description("IP address from which the challenge was created")]
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User agent string from the client.
    /// </summary>
    [Description("User agent string from the client")]
    public string? UserAgent { get; private set; }
    #endregion

    public static MfaChallenge Create(Guid id, Guid multiFactorAuthId, MfaMethod method,
        string challengeData, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(challengeData))
        {
            throw new MissingChallengeDataException();
        }

        if (expiresAt <= DateTime.UtcNow)
        {
            throw new InvalidExpirationTimeException();
        }

        var challenge = new MfaChallenge(id, multiFactorAuthId, method, challengeData, DateTime.UtcNow, expiresAt,
            ipAddress: ipAddress, userAgent: userAgent);
        return challenge;
    }

    public bool Verify(string response)
    {
        if (Status == MfaChallengeStatus.Verified)
        {
            return true; // Already verified
        }

        if (Status == MfaChallengeStatus.Expired)
        {
            return false; // Already expired
        }

        if (IsExpired())
        {
            Status = MfaChallengeStatus.Expired;
            return false;
        }

        AttemptCount++;

        var isValid = ValidateResponse(response);
        if (isValid)
        {
            Status = MfaChallengeStatus.Verified;
            VerifiedAt = DateTime.UtcNow;
        }

        return isValid;
    }

    public void Expire()
    {
        if (Status == MfaChallengeStatus.Verified)
        {
            return; // Already verified
        }

        Status = MfaChallengeStatus.Expired;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt || Status == MfaChallengeStatus.Expired;
    }

    public bool IsValid()
    {
        return Status == MfaChallengeStatus.Pending && !IsExpired();
    }

    private bool ValidateResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            return false;
        }

        // Method-specific validation
        return Method switch
        {
            MfaMethod.Totp => ValidateTotpResponse(response),
            MfaMethod.Sms => ValidateSmsResponse(response),
            MfaMethod.Email => ValidateEmailResponse(response),
            MfaMethod.Push => ValidatePushResponse(response),
            MfaMethod.Biometric => ValidateBiometricResponse(response),
            MfaMethod.HardwareKey => ValidateHardwareKeyResponse(response),
            MfaMethod.Voice => ValidateVoiceResponse(response),
            _ => false
        };
    }

    private bool ValidateTotpResponse(string response)
    {
        // TOTP validation - typically 6 digits
        return response.Length == 6 && response.All(char.IsDigit);
    }

    private bool ValidateSmsResponse(string response)
    {
        // SMS code validation - typically 4-8 digits
        return response.Length >= 4 && response.Length <= 8 && response.All(char.IsDigit);
    }

    private bool ValidateEmailResponse(string response)
    {
        // Email code validation - typically 6-8 alphanumeric characters
        return response.Length >= 6 && response.Length <= 8 && response.All(char.IsLetterOrDigit);
    }

    private bool ValidatePushResponse(string response)
    {
        // Push notification response - typically "approve" or "deny"
        return response.Equals("approve", StringComparison.OrdinalIgnoreCase);
    }

    private bool ValidateBiometricResponse(string response)
    {
        // Biometric validation - typically a token or signature
        return !string.IsNullOrWhiteSpace(response) && response.Length > 10;
    }

    private bool ValidateHardwareKeyResponse(string response)
    {
        // Hardware key validation - typically a signature or challenge response
        return !string.IsNullOrWhiteSpace(response) && response.Length > 20;
    }

    private bool ValidateVoiceResponse(string response)
    {
        // Voice call validation - typically a spoken code or phrase
        return !string.IsNullOrWhiteSpace(response) && response.Length >= 3;
    }
}