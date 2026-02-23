using System.Text.Json.Serialization;
using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents MFA configuration for an identity.
/// </summary>
public class MfaConfiguration : AggregateRoot<MfaConfigurationId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private MfaConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the MfaConfiguration aggregate root.
    /// </summary>
    /// <param name="id">The MFA configuration identifier.</param>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="method">The MFA method.</param>
    [JsonConstructor]
    public MfaConfiguration(
        MfaConfigurationId id,
        IdentityId identityId,
        MfaMethod method)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        Method = method;
        IsEnabled = false;
        CreatedAt = DateTime.UtcNow;
        
        AddEvent(new MfaConfigurationCreated(Id, IdentityId, Method, CreatedAt));
    }

    /// <summary>
    /// The identity identifier.
    /// </summary>
    public IdentityId IdentityId { get; private set; } = null!;

    /// <summary>
    /// The MFA method.
    /// </summary>
    public MfaMethod Method { get; private set; }

    /// <summary>
    /// Indicates whether this MFA method is enabled.
    /// </summary>
    public bool IsEnabled { get; private set; }

    /// <summary>
    /// The secret key for TOTP (if applicable).
    /// </summary>
    public string? SecretKey { get; private set; }

    /// <summary>
    /// The backup codes (hashed).
    /// </summary>
    public List<string> BackupCodes { get; private set; } = new();

    /// <summary>
    /// Date and time the configuration was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the configuration was enabled.
    /// </summary>
    public DateTime? EnabledAt { get; private set; }

    /// <summary>
    /// Date and time the configuration was last used.
    /// </summary>
    public DateTime? LastUsedAt { get; private set; }

    /// <summary>
    /// Enables this MFA method.
    /// </summary>
    /// <param name="secretKey">The secret key for TOTP (if applicable).</param>
    public void Enable(string? secretKey = null)
    {
        if (IsEnabled)
            return; // Already enabled
        
        IsEnabled = true;
        EnabledAt = DateTime.UtcNow;
        SecretKey = secretKey;
        
        AddEvent(new MfaConfigurationEnabled(Id, IdentityId, Method, EnabledAt.Value));
    }

    /// <summary>
    /// Disables this MFA method.
    /// </summary>
    public void Disable()
    {
        if (!IsEnabled)
            return; // Already disabled
        
        IsEnabled = false;
        SecretKey = null;
        BackupCodes.Clear();
        
        AddEvent(new MfaConfigurationDisabled(Id, IdentityId, Method, DateTime.UtcNow));
    }

    /// <summary>
    /// Sets the backup codes.
    /// </summary>
    /// <param name="backupCodes">The backup codes (should be hashed).</param>
    public void SetBackupCodes(List<string> backupCodes)
    {
        BackupCodes = backupCodes ?? new List<string>();
        IncrementVersion();
    }

    /// <summary>
    /// Updates the last used time.
    /// </summary>
    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
        IncrementVersion();
    }
}

