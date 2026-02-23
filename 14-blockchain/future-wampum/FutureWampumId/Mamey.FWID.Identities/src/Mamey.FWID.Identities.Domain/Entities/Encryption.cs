using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents an encryption aggregate root for managing data encryption policies.
/// </summary>
internal class Encryption : AggregateRoot<EncryptionId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Encryption()
    {
        EncryptedFields = new List<EncryptedField>();
        EncryptionKeys = new List<EncryptionKey>();
        AccessPolicies = new List<AccessPolicy>();
        AuditLog = new List<EncryptionEvent>();
    }

    /// <summary>
    /// Initializes a new instance of the Encryption aggregate root.
    /// </summary>
    /// <param name="id">The encryption identifier.</param>
    /// <param name="entityType">The type of entity being encrypted.</param>
    /// <param name="entityId">The identifier of the entity being encrypted.</param>
    /// <param name="encryptionLevel">The level of encryption required.</param>
    public Encryption(
        EncryptionId id,
        string entityType,
        string entityId,
        EncryptionLevel encryptionLevel)
        : base(id)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
        EncryptionLevel = encryptionLevel;
        Status = EncryptionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        EncryptedFields = new List<EncryptedField>();
        EncryptionKeys = new List<EncryptionKey>();
        AccessPolicies = new List<AccessPolicy>();
        AuditLog = new List<EncryptionEvent>();
        Version = 1;

        AddEvent(new EncryptionPolicyCreated(Id, EntityType, EntityId, EncryptionLevel, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The type of entity being encrypted.
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// The identifier of the entity being encrypted.
    /// </summary>
    public string EntityId { get; private set; }

    /// <summary>
    /// The level of encryption required.
    /// </summary>
    public EncryptionLevel EncryptionLevel { get; private set; }

    /// <summary>
    /// The current status of the encryption.
    /// </summary>
    public EncryptionStatus Status { get; private set; }

    /// <summary>
    /// When the encryption policy was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the encryption was last updated.
    /// </summary>
    public DateTime? LastUpdatedAt { get; private set; }

    /// <summary>
    /// The encryption algorithm used.
    /// </summary>
    public string? Algorithm { get; private set; }

    /// <summary>
    /// The key size in bits.
    /// </summary>
    public int? KeySize { get; private set; }

    /// <summary>
    /// The fields that are encrypted.
    /// </summary>
    public List<EncryptedField> EncryptedFields { get; private set; }

    /// <summary>
    /// The encryption keys used.
    /// </summary>
    public List<EncryptionKey> EncryptionKeys { get; private set; }

    /// <summary>
    /// The access policies for decryption.
    /// </summary>
    public List<AccessPolicy> AccessPolicies { get; private set; }

    /// <summary>
    /// The audit log of encryption operations.
    /// </summary>
    public List<EncryptionEvent> AuditLog { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Starts the encryption process.
    /// </summary>
    /// <param name="algorithm">The encryption algorithm to use.</param>
    /// <param name="keySize">The key size in bits.</param>
    public void StartEncryption(string algorithm, int keySize)
    {
        if (Status != EncryptionStatus.Pending)
            throw new InvalidOperationException("Encryption can only be started from pending status");

        Algorithm = algorithm;
        KeySize = keySize;
        Status = EncryptionStatus.Encrypting;
        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new EncryptionStarted(Id, algorithm, keySize, LastUpdatedAt.Value));
    }

    /// <summary>
    /// Adds an encrypted field.
    /// </summary>
    /// <param name="fieldName">The name of the field.</param>
    /// <param name="fieldType">The type of the field.</param>
    /// <param name="encryptedValue">The encrypted value.</param>
    /// <param name="keyId">The encryption key used.</param>
    public void AddEncryptedField(
        string fieldName,
        string fieldType,
        string encryptedValue,
        string keyId)
    {
        var field = new EncryptedField(
            fieldName,
            fieldType,
            encryptedValue,
            keyId,
            DateTime.UtcNow);

        EncryptedFields.Add(field);
        IncrementVersion();
    }

    /// <summary>
    /// Adds an encryption key.
    /// </summary>
    /// <param name="keyId">The key identifier.</param>
    /// <param name="keyType">The type of key.</param>
    /// <param name="keyStatus">The status of the key.</param>
    /// <param name="createdAt">When the key was created.</param>
    public void AddEncryptionKey(
        string keyId,
        KeyType keyType,
        KeyStatus keyStatus,
        DateTime createdAt)
    {
        var key = new EncryptionKey(
            keyId,
            keyType,
            keyStatus,
            createdAt);

        EncryptionKeys.Add(key);
        IncrementVersion();
    }

    /// <summary>
    /// Adds an access policy for decryption.
    /// </summary>
    /// <param name="policyId">The policy identifier.</param>
    /// <param name="userId">The user allowed to decrypt.</param>
    /// <param name="permissions">The permissions granted.</param>
    /// <param name="expiresAt">When the policy expires.</param>
    public void AddAccessPolicy(
        string policyId,
        string userId,
        List<string> permissions,
        DateTime? expiresAt = null)
    {
        var policy = new AccessPolicy(
            policyId,
            userId,
            permissions,
            expiresAt,
            DateTime.UtcNow);

        AccessPolicies.Add(policy);
        IncrementVersion();

        AddEvent(new AccessPolicyGranted(Id, policyId, userId, permissions, expiresAt));
    }

    /// <summary>
    /// Completes the encryption process.
    /// </summary>
    public void CompleteEncryption()
    {
        if (Status != EncryptionStatus.Encrypting)
            throw new InvalidOperationException("Encryption must be in progress to complete");

        Status = EncryptionStatus.Encrypted;
        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new EncryptionCompleted(Id, EncryptedFields.Count, LastUpdatedAt.Value));
    }

    /// <summary>
    /// Records a decryption access.
    /// </summary>
    /// <param name="userId">The user who accessed the data.</param>
    /// <param name="fieldName">The field that was decrypted.</param>
    /// <param name="purpose">The purpose of access.</param>
    public void RecordDecryptionAccess(string userId, string fieldName, string purpose)
    {
        var accessEvent = new EncryptionEvent(
            "decryption_access",
            userId,
            $"Field '{fieldName}' decrypted for '{purpose}'",
            DateTime.UtcNow);

        AuditLog.Add(accessEvent);
        IncrementVersion();

        AddEvent(new DataAccessed(Id, userId, fieldName, purpose, DateTime.UtcNow));
    }

    /// <summary>
    /// Rotates encryption keys.
    /// </summary>
    /// <param name="oldKeyId">The old key identifier.</param>
    /// <param name="newKeyId">The new key identifier.</param>
    /// <param name="rotatedBy">The user who performed the rotation.</param>
    public void RotateKey(string oldKeyId, string newKeyId, string rotatedBy)
    {
        // Mark old key as rotated
        var oldKey = EncryptionKeys.FirstOrDefault(k => k.KeyId == oldKeyId);
        if (oldKey != null)
        {
            oldKey.Status = KeyStatus.Rotated;
        }

        // Add new key
        AddEncryptionKey(newKeyId, KeyType.DataEncryption, KeyStatus.Active, DateTime.UtcNow);

        LastUpdatedAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new KeyRotated(Id, oldKeyId, newKeyId, rotatedBy, LastUpdatedAt.Value));
    }

    /// <summary>
    /// Checks if a user has access to decrypt data.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="requiredPermission">The required permission.</param>
    /// <returns>True if the user has access.</returns>
    public bool HasAccess(string userId, string requiredPermission)
    {
        return AccessPolicies.Any(p =>
            p.UserId == userId &&
            p.Permissions.Contains(requiredPermission) &&
            (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow));
    }

    /// <summary>
    /// Gets the active encryption key.
    /// </summary>
    /// <returns>The active encryption key.</returns>
    public EncryptionKey? GetActiveKey()
    {
        return EncryptionKeys.FirstOrDefault(k => k.Status == KeyStatus.Active);
    }

    /// <summary>
    /// Checks if the encryption is compliant with the required level.
    /// </summary>
    /// <returns>True if compliant.</returns>
    public bool IsCompliant()
    {
        if (Status != EncryptionStatus.Encrypted)
            return false;

        // Check algorithm strength based on encryption level
        switch (EncryptionLevel)
        {
            case EncryptionLevel.Standard:
                return Algorithm == "AES256" && (KeySize >= 256);
            case EncryptionLevel.High:
                return Algorithm == "AES256" && (KeySize >= 256) && HasHsmKeys();
            case EncryptionLevel.Critical:
                return Algorithm == "AES256" && (KeySize >= 256) && HasHsmKeys() && HasMultiKeyEncryption();
            default:
                return false;
        }
    }

    private bool HasHsmKeys()
    {
        return EncryptionKeys.Any(k => k.KeyType == KeyType.HsmStored);
    }

    private bool HasMultiKeyEncryption()
    {
        return EncryptionKeys.Count(k => k.Status == KeyStatus.Active) >= 2;
    }

    #endregion
}

/// <summary>
/// Represents the level of encryption required.
/// </summary>
internal enum EncryptionLevel
{
    Standard,   // AES256 with software keys
    High,       // AES256 with HSM keys
    Critical    // Multi-key encryption with HSM
}

/// <summary>
/// Represents the status of encryption.
/// </summary>
internal enum EncryptionStatus
{
    Pending,
    Encrypting,
    Encrypted,
    Failed
}

/// <summary>
/// Represents an encrypted field.
/// </summary>
internal class EncryptedField
{
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public string EncryptedValue { get; set; }
    public string KeyId { get; set; }
    public DateTime EncryptedAt { get; set; }

    public EncryptedField(
        string fieldName,
        string fieldType,
        string encryptedValue,
        string keyId,
        DateTime encryptedAt)
    {
        FieldName = fieldName;
        FieldType = fieldType;
        EncryptedValue = encryptedValue;
        KeyId = keyId;
        EncryptedAt = encryptedAt;
    }
}

/// <summary>
/// Represents an encryption key.
/// </summary>
internal class EncryptionKey
{
    public string KeyId { get; set; }
    public KeyType KeyType { get; set; }
    public KeyStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public EncryptionKey(
        string keyId,
        KeyType keyType,
        KeyStatus status,
        DateTime createdAt)
    {
        KeyId = keyId;
        KeyType = keyType;
        Status = status;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// Represents the type of encryption key.
/// </summary>
internal enum KeyType
{
    DataEncryption,
    KeyEncryption,
    HsmStored,
    CloudKms
}

/// <summary>
/// Represents the status of an encryption key.
/// </summary>
internal enum KeyStatus
{
    Active,
    Inactive,
    Rotated,
    Compromised
}

/// <summary>
/// Represents an access policy for decryption.
/// </summary>
internal class AccessPolicy
{
    public string PolicyId { get; set; }
    public string UserId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime? ExpiresAt { get; set; }
    public DateTime GrantedAt { get; set; }

    public AccessPolicy(
        string policyId,
        string userId,
        List<string> permissions,
        DateTime? expiresAt,
        DateTime grantedAt)
    {
        PolicyId = policyId;
        UserId = userId;
        Permissions = permissions;
        ExpiresAt = expiresAt;
        GrantedAt = grantedAt;
    }
}

/// <summary>
/// Represents an encryption-related event.
/// </summary>
internal class EncryptionEvent
{
    public string EventType { get; set; }
    public string UserId { get; set; }
    public string Description { get; set; }
    public DateTime OccurredAt { get; set; }

    public EncryptionEvent(
        string eventType,
        string userId,
        string description,
        DateTime occurredAt)
    {
        EventType = eventType;
        UserId = userId;
        Description = description;
        OccurredAt = occurredAt;
    }
}
