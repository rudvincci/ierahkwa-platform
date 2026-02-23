using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Mamey.Identity.EntityFramework.Entities;
using System.Security.Claims;

namespace Mamey.Identity.EntityFramework.Stores;

public class MameyClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
    public MameyClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Add custom claims
        if (!string.IsNullOrEmpty(user.FullName))
            identity.AddClaim(new Claim("full_name", user.FullName));

        if (user.TenantId.HasValue)
            identity.AddClaim(new Claim("tenant_id", user.TenantId.Value.ToString()));

        if (user.CreatedAt != default)
            identity.AddClaim(new Claim("created_at", user.CreatedAt.ToString("O")));

        if (user.LastLoginAt.HasValue)
            identity.AddClaim(new Claim("last_login_at", user.LastLoginAt.Value.ToString("O")));

        identity.AddClaim(new Claim("is_active", user.IsActive.ToString()));

        if (!string.IsNullOrEmpty(user.TimeZone))
            identity.AddClaim(new Claim("time_zone", user.TimeZone));

        if (!string.IsNullOrEmpty(user.Locale))
            identity.AddClaim(new Claim("locale", user.Locale));

        return identity;
    }
}

