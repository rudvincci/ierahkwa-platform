using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mamey.Identity.EntityFramework.Entities;
using Mamey.Identity.EntityFramework.DbContexts;

namespace Mamey.Identity.EntityFramework.Stores;

public class MameyRoleStore<TContext> : RoleStore<ApplicationRole, TContext, string>
    where TContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public MameyRoleStore(TContext context, IdentityErrorDescriber? describer = null)
        : base(context, describer)
    {
    }

    public override async Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Roles
            .Include(r => r.RoleClaims)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public override async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        return await Roles
            .Include(r => r.RoleClaims)
            .FirstOrDefaultAsync(r => r.NormalizedName == normalizedRoleName, cancellationToken);
    }
}
