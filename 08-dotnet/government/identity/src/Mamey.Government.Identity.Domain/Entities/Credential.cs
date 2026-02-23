using System.ComponentModel;
using Mamey.Government.Identity.Domain.Events;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Types;

namespace Mamey.Government.Identity.Domain.Entities;

internal class Credential : AggregateRoot<CredentialId>
{
    public Credential(CredentialId id, UserId userId, CredentialType type, string value,
        DateTime createdAt, DateTime? modifiedAt = null, DateTime? expiresAt = null,
        CredentialStatus status = CredentialStatus.Active, int version = 0)
        : base(id, version)
    {
        UserId = userId;
        Type = type;
        Value = value;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
        ExpiresAt = expiresAt;
        Status = status;
    }

    #region Properties

    /// <summary>
    /// Reference to the user this credential belongs to.
    /// </summary>
    [Description("Reference to the user this credential belongs to")]
    public UserId UserId { get; private set; }

    /// <summary>
    /// Type of credential (password, biometric, etc.).
    /// </summary>
    [Description("Type of credential")]
    public CredentialType Type { get; private set; }

    /// <summary>
    /// Encrypted or hashed value of the credential.
    /// </summary>
    [Description("Encrypted or hashed value of the credential")]
    public string Value { get; private set; }

    /// <summary>
    /// Date and time the credential was created.
    /// </summary>
    [Description("Date and time the credential was created")]
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the credential was modified.
    /// </summary>
    [Description("Date and time the credential was modified")]
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Date and time the credential expires (if applicable).
    /// </summary>
    [Description("Date and time the credential expires")]
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>
    /// Current status of the credential.
    /// </summary>
    [Description("Current status of the credential")]
    public CredentialStatus Status { get; private set; }

    /// <summary>
    /// Date and time the credential was last used.
    /// </summary>
    [Description("Date and time the credential was last used")]
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Number of times the credential has been used.
    /// </summary>
    [Description("Number of times the credential has been used")]
    public int UsageCount { get; private set; }
    #endregion

    public static Credential Create(Guid id, Guid userId, CredentialType type, string value, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new MissingCredentialValueException();
        }

        if (expiresAt.HasValue && expiresAt.Value <= DateTime.UtcNow)
        {
            throw new InvalidCredentialExpirationException();
        }

        var credential = new Credential(id, userId, type, value, DateTime.UtcNow, expiresAt: expiresAt);
        credential.AddEvent(new CredentialCreated(credential));
        return credential;
    }

    public void Update(string newValue, DateTime? newExpiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            throw new MissingCredentialValueException();
        }

        if (newExpiresAt.HasValue && newExpiresAt.Value <= DateTime.UtcNow)
        {
            throw new InvalidCredentialExpirationException();
        }

        Value = newValue;
        ExpiresAt = newExpiresAt;
        ModifiedAt = DateTime.UtcNow;
        
        AddEvent(new CredentialUpdated(this));
    }

    public void Activate()
    {
        if (Status == CredentialStatus.Active)
        {
            throw new CredentialAlreadyActiveException();
        }

        Status = CredentialStatus.Active;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new CredentialActivated(this));
    }

    public void Deactivate()
    {
        if (Status == CredentialStatus.Inactive)
        {
            throw new CredentialAlreadyInactiveException();
        }

        Status = CredentialStatus.Inactive;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new CredentialDeactivated(this));
    }

    public void Revoke()
    {
        if (Status == CredentialStatus.Revoked)
        {
            throw new CredentialAlreadyRevokedException();
        }

        Status = CredentialStatus.Revoked;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new CredentialRevoked(this));
    }

    public void RecordUsage()
    {
        if (Status != CredentialStatus.Active)
        {
            throw new CredentialNotActiveException();
        }

        LastUsedAt = DateTime.UtcNow;
        UsageCount++;
        ModifiedAt = DateTime.UtcNow;
        
        AddEvent(new CredentialUsed(this));
    }

    public void Expire()
    {
        if (Status == CredentialStatus.Expired)
        {
            throw new CredentialAlreadyExpiredException();
        }

        Status = CredentialStatus.Expired;
        ModifiedAt = DateTime.UtcNow;
        AddEvent(new CredentialExpired(this));
    }

    public bool IsExpired()
    {
        return ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value || Status == CredentialStatus.Expired;
    }

    public bool IsActive()
    {
        return Status == CredentialStatus.Active && !IsExpired();
    }

    public bool CanBeUsed()
    {
        return IsActive();
    }
}