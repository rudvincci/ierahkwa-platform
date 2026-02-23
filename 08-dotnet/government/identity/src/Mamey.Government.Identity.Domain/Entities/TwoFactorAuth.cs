using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class TwoFactorAuth : AggregateRoot<TwoFactorAuthId>
{
    public TwoFactorAuth(TwoFactorAuthId id, UserId userId, string secretKey, string qrCodeUrl,
        DateTime createdAt, DateTime? activatedAt = null, TwoFactorAuthStatus status = TwoFactorAuthStatus.Pending,
        int version = 0)
        : base(id, version)
    {
        UserId = userId;
        SecretKey = secretKey;
        QrCodeUrl = qrCodeUrl;
        CreatedAt = createdAt;
        ActivatedAt = activatedAt;
        Status = status;
    }

    #region Properties

    /// <summary>
    /// Reference to the user this 2FA belongs to.
    /// </summary>
    [Description("Reference to the user this 2FA belongs to")]
    public UserId UserId { get; private set; }

    /// <summary>
    /// Secret key for TOTP generation.
    /// </summary>
    [Description("Secret key for TOTP generation")]
    public string SecretKey { get; private set; }

    /// <summary>
    /// QR code URL for setup in authenticator apps.
    /// </summary>
    [Description("QR code URL for setup in authenticator apps")]
    public string QrCodeUrl { get; private set; }

    /// <summary>
    /// Date and time the 2FA was created.
    /// </summary>
    [Description("Date and time the 2FA was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the 2FA was last modified.
    /// </summary>
    [Description("Date and time the 2FA was last modified")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the 2FA was activated.
    /// </summary>
    [Description("Date and time the 2FA was activated")]
    public DateTime? ActivatedAt { get; private set; }

    /// <summary>
    /// Current status of the 2FA.
    /// </summary>
    [Description("Current status of the 2FA")]
    public TwoFactorAuthStatus Status { get; private set; }

    /// <summary>
    /// Date and time the 2FA was last used.
    /// </summary>
    [Description("Date and time the 2FA was last used")]
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Number of times the 2FA has been used.
    /// </summary>
    [Description("Number of times the 2FA has been used")]
    public int UsageCount { get; private set; }

    /// <summary>
    /// Number of failed verification attempts.
    /// </summary>
    [Description("Number of failed verification attempts")]
    public int FailedAttempts { get; private set; }

    /// <summary>
    /// Date and time the 2FA was last failed.
    /// </summary>
    [Description("Date and time the 2FA was last failed")]
    public DateTime? LastFailedAt { get; private set; }

    /// <summary>
    /// Backup codes for recovery.
    /// </summary>
    [Description("Backup codes for recovery")]
    public IReadOnlyList<string> BackupCodes { get; private set; } = new List<string>();
    #endregion

    public static TwoFactorAuth Create(Guid id, Guid userId, string secretKey, string qrCodeUrl)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new MissingSecretKeyException();
        }

        if (string.IsNullOrWhiteSpace(qrCodeUrl))
        {
            throw new MissingQrCodeUrlException();
        }

        var twoFactorAuth = new TwoFactorAuth(id, userId, secretKey, qrCodeUrl, DateTime.UtcNow);
        twoFactorAuth.AddEvent(new TwoFactorAuthCreated(twoFactorAuth));
        return twoFactorAuth;
    }

    public void Activate(string totpCode)
    {
        if (Status == TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorAuthAlreadyActiveException();
        }

        if (Status == TwoFactorAuthStatus.Disabled)
        {
            throw new TwoFactorAuthDisabledException();
        }

        if (!VerifyTotpCode(totpCode))
        {
            FailedAttempts++;
            LastFailedAt = DateTime.UtcNow;
            AddEvent(new TwoFactorAuthActivationFailed(this, FailedAttempts));
            throw new InvalidTotpCodeException();
        }

        Status = TwoFactorAuthStatus.Active;
        ActivatedAt = DateTime.UtcNow;
        FailedAttempts = 0;
        AddEvent(new TwoFactorAuthActivated(this));
    }

    public void Verify(string totpCode)
    {
        if (Status != TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorAuthNotActiveException();
        }

        if (!VerifyTotpCode(totpCode))
        {
            FailedAttempts++;
            LastFailedAt = DateTime.UtcNow;
            AddEvent(new TwoFactorAuthVerificationFailed(this, FailedAttempts));
            throw new InvalidTotpCodeException();
        }

        LastUsedAt = DateTime.UtcNow;
        UsageCount++;
        FailedAttempts = 0;
        AddEvent(new TwoFactorAuthVerified(this));
    }

    public void Disable()
    {
        if (Status == TwoFactorAuthStatus.Disabled)
        {
            throw new TwoFactorAuthAlreadyDisabledException();
        }

        Status = TwoFactorAuthStatus.Disabled;
        AddEvent(new TwoFactorAuthDisabled(this));
    }

    public void Enable()
    {
        if (Status == TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorAuthAlreadyActiveException();
        }

        Status = TwoFactorAuthStatus.Pending;
        AddEvent(new TwoFactorAuthEnabled(this));
    }

    public void GenerateBackupCodes(int count = 10)
    {
        if (Status != TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorAuthNotActiveException();
        }

        var codes = new List<string>();
        for (int i = 0; i < count; i++)
        {
            codes.Add(GenerateBackupCode());
        }

        BackupCodes = codes;
        AddEvent(new TwoFactorAuthBackupCodesGenerated(this, codes));
    }

    public bool VerifyBackupCode(string backupCode)
    {
        if (Status != TwoFactorAuthStatus.Active)
        {
            return false;
        }

        var isValid = BackupCodes.Contains(backupCode);
        if (isValid)
        {
            LastUsedAt = DateTime.UtcNow;
            UsageCount++;
            AddEvent(new TwoFactorAuthBackupCodeUsed(this, backupCode));
        }
        else
        {
            FailedAttempts++;
            LastFailedAt = DateTime.UtcNow;
            AddEvent(new TwoFactorAuthBackupCodeFailed(this, FailedAttempts));
        }

        return isValid;
    }

    public bool IsLocked()
    {
        return FailedAttempts >= 5 && LastFailedAt.HasValue && 
               DateTime.UtcNow < LastFailedAt.Value.AddMinutes(15);
    }

    public bool CanBeUsed()
    {
        return Status == TwoFactorAuthStatus.Active && !IsLocked();
    }

    public void RegenerateSecretKey()
    {
        if (Status != TwoFactorAuthStatus.Active)
        {
            throw new TwoFactorNotActiveException();
        }

        SecretKey = GenerateSecretKey();
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new TwoFactorAuthSecretKeyRegenerated(this));
    }

    private bool VerifyTotpCode(string totpCode)
    {
        // This would typically use a TOTP library like OtpNet
        // For now, we'll implement a basic validation
        if (string.IsNullOrWhiteSpace(totpCode) || totpCode.Length != 6)
        {
            return false;
        }

        // In a real implementation, this would verify against the secret key
        // using the current time window
        return totpCode.All(char.IsDigit);
    }

    private string GenerateBackupCode()
    {
        // Generate a random 8-character backup code
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private string GenerateSecretKey()
    {
        // Generate a random 32-character base32 secret key
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 32)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}