using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.DTO;

namespace Mamey.Government.Modules.Identity.Core.Mappings;

internal static class UserProfileMappings
{
    public static UserProfileDto AsDto(this UserProfile userProfile)
        => new()
        {
            Id = userProfile.Id.Value,
            AuthenticatorIssuer = userProfile.AuthenticatorIssuer,
            AuthenticatorSubject = userProfile.AuthenticatorSubject,
            Email = userProfile.Email,
            DisplayName = userProfile.DisplayName,
            TenantId = userProfile.TenantId,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt,
            LastLoginAt = userProfile.LastLoginAt
        };

    public static UserProfileSummaryDto AsSummaryDto(this UserProfile userProfile)
        => new()
        {
            Id = userProfile.Id.Value,
            Email = userProfile.Email,
            DisplayName = userProfile.DisplayName,
            TenantId = userProfile.TenantId,
            LastLoginAt = userProfile.LastLoginAt
        };
}
