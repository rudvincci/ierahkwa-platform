using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Domain.Entities;

/// <summary>
/// User session entity for tracking active sessions.
/// Stored in Redis for fast access.
/// </summary>
internal class UserSession
{
    public UserSession(
        UserId userId,
        string sessionId,
        string authenticatorIssuer,
        string authenticatorSubject,
        Guid? tenantId)
    {
        UserId = userId;
        SessionId = sessionId;
        AuthenticatorIssuer = authenticatorIssuer;
        AuthenticatorSubject = authenticatorSubject;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
        LastAccessedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddHours(24); // 24 hour session
    }

    public UserId UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string AuthenticatorIssuer { get; set; } = string.Empty;
    public string AuthenticatorSubject { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive => DateTime.UtcNow < ExpiresAt;

    public void Refresh()
    {
        LastAccessedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddHours(24);
    }
}
