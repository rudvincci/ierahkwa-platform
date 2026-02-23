using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;

/// <summary>
/// Token entity for resuming citizenship applications.
/// Stores hashed tokens that allow users to resume their application via email link.
/// </summary>
internal class ApplicationToken
{
    private ApplicationToken() { } // For EF Core

    public ApplicationToken(
        Guid id,
        AppId applicationId,
        string tokenHash,
        string email,
        DateTime expiresAt)
    {
        Id = id;
        ApplicationId = applicationId;
        TokenHash = tokenHash;
        Email = email;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        UsedAt = null;
    }

    public Guid Id { get; private set; }
    public AppId ApplicationId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsUsed => UsedAt.HasValue;
    public bool IsValid => !IsExpired && !IsUsed;

    public void MarkAsUsed()
    {
        if (IsUsed)
            throw new InvalidOperationException("Token has already been used.");

        UsedAt = DateTime.UtcNow;
    }
}
