using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Mongo.Documents;

internal class UserProfileDocument : MicroMonolith.Infrastructure.Mongo.IIdentifiable<Guid>
{
    public UserProfileDocument()
    {
    }

    public UserProfileDocument(UserProfile userProfile)
    {
        Id = userProfile.Id.Value;
        AuthenticatorIssuer = userProfile.AuthenticatorIssuer;
        AuthenticatorSubject = userProfile.AuthenticatorSubject;
        Email = userProfile.Email;
        DisplayName = userProfile.DisplayName;
        TenantId = userProfile.TenantId;
        CreatedAt = userProfile.CreatedAt;
        UpdatedAt = userProfile.UpdatedAt;
        LastLoginAt = userProfile.LastLoginAt;
    }

    public Guid Id { get; set; }
    public string AuthenticatorIssuer { get; set; } = string.Empty;
    public string AuthenticatorSubject { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public UserProfile AsEntity()
    {
        var userId = new UserId(Id);
        var profile = new UserProfile(
            userId,
            AuthenticatorIssuer,
            AuthenticatorSubject,
            Email,
            DisplayName,
            TenantId);
        
        // Use reflection to set private properties
        typeof(UserProfile).GetProperty("CreatedAt")?.SetValue(profile, CreatedAt);
        typeof(UserProfile).GetProperty("UpdatedAt")?.SetValue(profile, UpdatedAt);
        typeof(UserProfile).GetProperty("LastLoginAt")?.SetValue(profile, LastLoginAt);
        
        return profile;
    }
}
