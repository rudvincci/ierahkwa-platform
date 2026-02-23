using Mamey.Government.Modules.Identity.Core.Domain.Events;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Domain.Entities;

/// <summary>
/// User profile entity for local user data storage.
/// Authentik handles authentication, this stores profile information.
/// </summary>
public class UserProfile : AggregateRoot<UserId>
{
    private UserProfile() { }

    public UserProfile(
        UserId id,
        string authenticatorIssuer,
        string authenticatorSubject,
        string? email = null,
        string? displayName = null,
        Guid? tenantId = null,
        int version = 0)
        : base(id, version)
    {
        AuthenticatorIssuer = authenticatorIssuer;
        AuthenticatorSubject = authenticatorSubject;
        Email = email;
        DisplayName = displayName;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new UserProfileCreated(Id, AuthenticatorIssuer, AuthenticatorSubject));
    }

    public string AuthenticatorIssuer { get; private set; } = string.Empty;
    public string AuthenticatorSubject { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? DisplayName { get; private set; }
    public Guid? TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public void UpdateProfile(string? email, string? displayName)
    {
        Email = email;
        DisplayName = displayName;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new UserProfileModified(this));
    }

    public void UpdateTenant(Guid? tenantId)
    {
        TenantId = tenantId;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new UserProfileModified(this));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
