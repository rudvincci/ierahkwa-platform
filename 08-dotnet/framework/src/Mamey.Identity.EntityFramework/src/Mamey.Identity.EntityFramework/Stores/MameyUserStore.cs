using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mamey.Identity.EntityFramework.Entities;
using Mamey.Identity.EntityFramework.DbContexts;
using System.Security.Claims;

namespace Mamey.Identity.EntityFramework.Stores;

public class MameyUserStore<TContext> : UserStore<ApplicationUser, ApplicationRole, TContext, string>
    where TContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public MameyUserStore(TContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    public override async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public override async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public override async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserClaims)
            .FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var claims = await base.GetClaimsAsync(user, cancellationToken);

        // Add custom claims
        if (!string.IsNullOrEmpty(user.FullName))
            claims.Add(new Claim("full_name", user.FullName));

        if (user.TenantId.HasValue)
            claims.Add(new Claim("tenant_id", user.TenantId.Value.ToString()));

        if (user.CreatedAt != default)
            claims.Add(new Claim("created_at", user.CreatedAt.ToString("O")));

        if (user.LastLoginAt.HasValue)
            claims.Add(new Claim("last_login_at", user.LastLoginAt.Value.ToString("O")));

        claims.Add(new Claim("is_active", user.IsActive.ToString()));

        if (!string.IsNullOrEmpty(user.TimeZone))
            claims.Add(new Claim("time_zone", user.TimeZone));

        if (!string.IsNullOrEmpty(user.Locale))
            claims.Add(new Claim("locale", user.Locale));

        return claims;
    }
}
