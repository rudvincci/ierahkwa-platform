using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mamey.ApplicationName.Modules.Identity.Core.EF;
using Mamey.Auth.Identity;    // for ClaimCategory & ClaimValues
using Mamey.Auth.Identity.Entities;
using Mamey.Types;
using Microsoft.AspNetCore.Identity;

internal sealed class IdentityInitializer : IInitializer
{
    private readonly ApplicationIdentityDbContext _db;
    private readonly IPasswordHasher<ApplicationUser> _hasher;
    private readonly ILogger<IdentityInitializer> _log;
    private readonly ApplicationIdentityUnitOfWork _unitOfWork;

    private static readonly Guid SystemUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid DaemonUserId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid AdminUserId  = Guid.Parse("00000000-0000-0000-0000-000000000003");

    private const string SystemEmail = "system@local.com";
    private const string DaemonEmail = "daemon@local.com";
    private const string AdminEmail  = "admin@example.com";

    private const string SystemPass = "System@123";
    private const string DaemonPass = "Daemon@123";
    private const string AdminPass  = "Admin@123";

    private const string RoleAdmin = ClaimValues.Role.Admin;
    private const string RoleTenantAdmin = ClaimValues.Role.TenantAdmin;
    private const string RoleSupport     = ClaimValues.Role.Support;
    private const string RoleUser        = ClaimValues.Role.User;
    private const string RoleDaemon      = ClaimValues.Role.Daemon;

    public IdentityInitializer(
        ApplicationIdentityDbContext db,
        IPasswordHasher<ApplicationUser> hasher,
        ILogger<IdentityInitializer> log,
        ApplicationIdentityUnitOfWork unitOfWork)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _log = log ?? throw new ArgumentNullException(nameof(log));
        _unitOfWork = unitOfWork;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        _log.LogInformation("🔧 Initializing Identity module");

        await _db.Database.MigrateAsync(ct);
        _log.LogInformation("✅ Database migrations applied");

        await _unitOfWork.ExecuteAsync(async () =>
        {
            await SeedPermissionsAsync(ct);
            await SeedRolesAsync(ct);
            await SeedUsersAsync(ct);

            _log.LogInformation("✅ Identity seeding complete");
        });
    }

    private async Task SeedPermissionsAsync(CancellationToken ct)
    {
        var existing = await _db.Permissions
            .Select(p => p.Name)
            .ToListAsync(ct);

        var perms = new List<Permission>
        {
            new Permission(Guid.NewGuid(), ClaimValues.Permission.IdentityRead,   "Read identity data",    ClaimCategory.Permission,   1),
            new Permission(Guid.NewGuid(), ClaimValues.Permission.IdentityWrite,  "Write identity data",   ClaimCategory.Permission,   2),
            new Permission(Guid.NewGuid(), ClaimValues.Permission.IdentityAdmin,  "Full identity control", ClaimCategory.Permission,   3),
            new Permission(Guid.NewGuid(), ClaimValues.Permission.SystemAll,      "System level control",  ClaimCategory.Permission,  99),
            new Permission(Guid.NewGuid(), ClaimValues.Permission.DaemonExecute,  "Daemon execution",      ClaimCategory.Permission,  50),
        };

        var toAdd = perms.Where(p => !existing.Contains(p.Name)).ToList();
        if (toAdd.Any())
        {
            _db.Permissions.AddRange(toAdd);
            await _db.SaveChangesAsync(ct);
            _log.LogInformation("🔑 Seeded {Count} permissions", toAdd.Count);
        }
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        var defs = new[]
        {
            new RoleDef(RoleAdmin, true,  "System super user",      new[] { ClaimValues.Permission.SystemAll,     ClaimValues.Permission.IdentityAdmin }),
            new RoleDef(RoleTenantAdmin, false,"Tenant administrator",    new[] { ClaimValues.Permission.IdentityAdmin, ClaimValues.Permission.IdentityRead, ClaimValues.Permission.IdentityWrite }),
            new RoleDef(RoleSupport,     false,"Support staff",          new[] { ClaimValues.Permission.IdentityRead }),
            new RoleDef(RoleUser,        false,"Regular user",           new[] { ClaimValues.Permission.IdentityRead }),
            new RoleDef(RoleDaemon,      true, "Background daemon",       new[] { ClaimValues.Permission.DaemonExecute }),
        };

        foreach (var def in defs)
        {
            var norm = def.Name.ToUpperInvariant();
            var role = await _db.Roles
                .IgnoreQueryFilters()           // include system roles too
                .FirstOrDefaultAsync(r => r.NormalizedName == norm, ct);

            if (role == null)
            {
                role = new ApplicationRole(
                    id:           RoleId.New(),
                    name:         def.Name,
                    createdBy:    SystemUserId,
                    isSystemRole: def.IsSystem,
                    description:  def.Description
                );
                role.SetAudit(SystemUserId);

                _db.Roles.Add(role);
                await _db.SaveChangesAsync(ct);
                _log.LogInformation("➕ Created role '{Role}'", def.Name);
            }

            var existingClaims = await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(ct);

            foreach (var perm in def.PermissionClaims.Except(existingClaims))
            {
                _db.RoleClaims.Add(new ApplicationRoleClaim(
                    Guid.NewGuid(),
                    role.Id,
                    ClaimCategory.Permission,
                    perm,
                    ClaimCategory.Permission,
                    SystemUserId
                ));
            }

            await _db.SaveChangesAsync(ct);
        }

        _log.LogInformation("✅ Seeded roles and role‐claims");
    }

    private async Task SeedUsersAsync(CancellationToken ct)
    {
        await EnsureUserAsync(
            AdminUserId,  AdminEmail,  "admin",  new Name("System","Administrator"), AdminPass,  new[]{ RoleAdmin }, false, null, ct);
        await EnsureUserAsync(
            SystemUserId, SystemEmail, "system", new Name("System","Account"),       SystemPass, new[]{ RoleAdmin }, true,  new[]{ new Claim(ClaimTypes.Role,RoleAdmin) }, ct);
        await EnsureUserAsync(
            DaemonUserId, DaemonEmail, "daemon", new Name("Daemon","Service"),       DaemonPass, new[]{ RoleDaemon      }, true,  new[]{ new Claim(ClaimTypes.Role,RoleDaemon)      }, ct);

    #if DEBUG
        foreach (var s in GetSeedStaffUsers())
            await EnsureUserAsync(s.Id, s.Email, s.Username, s.Name, s.Password, s.Roles, false, s.Claims, ct);

        foreach (var u in GetSeedRegularUsers())
            await EnsureUserAsync(u.Id, u.Email, u.Username, u.Name, u.Password, u.Roles, false, null, ct);
    #endif
    }

    private async Task EnsureUserAsync(
        Guid userId,
        string email,
        string userName,
        Name name,
        string password,
        IEnumerable<string> roleNames,
        bool isSystem,
        IEnumerable<Claim>? extraClaims,
        CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
        {
            user = new ApplicationUser(
                id:                userId,
                userName:          userName,
                name:              name,
                email:             new Email(email),
                isExternalLogin:   false,
                description:       isSystem ? "System account" : "Seeded account",
                isSystem:          isSystem
            );
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.PasswordHash  = _hasher.HashPassword(user,password);
            user.SetAudit(SystemUserId);

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
            _log.LogInformation("➕ Created user '{User}'", userName);
        }

        // Assign missing roles
        var currentRoles = await _db.UserRoles
            .IgnoreQueryFilters()              // ← include system roles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.NormalizedName)
            .ToListAsync(ct);

        foreach (var rn in roleNames.Select(r => r.ToUpperInvariant()).Except(currentRoles))
        {
            var r = await _db.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.NormalizedName == rn, ct);
            if (r == null)
            {
                _log.LogWarning("Skipped assigning missing role '{Role}' to '{User}'", rn, userName);
                continue;
            }
            _log.LogDebug("Assigning role '{Role}' to '{User}'", rn, userName);
            _db.UserRoles.Add(new ApplicationUserRole(userId, r.Id));
        }

        await _db.SaveChangesAsync(ct);

        // Assign missing claims
        var existing = await _db.UserClaims
            .Where(uc => uc.UserId == userId)
            .Select(uc => new { uc.ClaimType, uc.ClaimValue, uc.Category })
            .ToListAsync(ct);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,  name.ToString()),
            new Claim(ClaimTypes.Email, email)
        };
        if (extraClaims != null) claims.AddRange(extraClaims);

        foreach (var c in claims)
        {
            var cat = c.Type switch
            {
                ClaimTypes.Name  => ClaimCategory.Profile,
                ClaimTypes.Email => ClaimCategory.Profile,
                ClaimTypes.Role  => ClaimCategory.Role,
                _                => ClaimCategory.System
            };
            if (!existing.Any(ec => ec.ClaimType == c.Type
                                 && ec.ClaimValue == c.Value
                                 && ec.Category == cat))
            {
                _db.UserClaims.Add(new ApplicationUserClaim(
                    Guid.NewGuid(),
                    userId,
                    c.Type,
                    c.Value,
                    cat,
                    SystemUserId
                ));
            }
        }

        await _db.SaveChangesAsync(ct);
    }

#if DEBUG
    private IEnumerable<(UserId Id,string Email,string Username,Name Name,string Password,string[] Roles,Claim[] Claims)> GetSeedStaffUsers() => new[]
    {
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000101")),
         "maria.support@example.com","maria.support",new Name("Maria","Support"),"Support@123",
         new[]{RoleSupport},new[]{new Claim("Department","Support"),new Claim("AccessLevel","ReadOnly")}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000102")),
         "john.admin@example.com","john.admin",new Name("John","Admin"),"Admin@123",
         new[]{RoleTenantAdmin},new[]{new Claim("Department","Admin"),new Claim("AccessLevel","Full")}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000103")),
         "aisha.hr@example.com","aisha.hr",new Name("Aisha","HR"),"HR@123",
         new[]{RoleSupport},new[]{new Claim("Department","HR"),new Claim("AccessLevel","Restricted")}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000104")),
         "chloe.it@example.com","chloe.it",new Name("Chloe","IT"),"IT@123",
         new[]{RoleSupport},new[]{new Claim("Department","IT"),new Claim("AccessLevel","InternalOnly")}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000105")),
         "david.finance@example.com","david.finance",new Name("David","Finance"),"Finance@123",
         new[]{RoleSupport},new[]{new Claim("Department","Finance"),new Claim("AccessLevel","Sensitive")}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000106")),
         "nina.ops@example.com","nina.ops",new Name("Nina","Ops"),"Ops@123",
         new[]{RoleSupport},new[]{new Claim("Department","Operations"),new Claim("AccessLevel","Full")}),
    };

    private IEnumerable<(UserId Id,string Email,string Username,Name Name,string Password,string[] Roles)> GetSeedRegularUsers() => new[]
    {
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000201")),
         "jane.user@example.com","jane.user",new Name("Jane","User"),"User@123",new[]{RoleUser}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000202")),
         "kofi.user@example.com","kofi.user",new Name("Kofi","User"),"User@123",new[]{RoleUser}),
        (new UserId(Guid.Parse("00000000-0000-0000-0000-000000000203")),
         "lucia.reader@example.com","lucia.reader",new Name("Lucia","Reader"),"User@123",new[]{RoleUser}),
    };
#endif

    private sealed record RoleDef(string Name, bool IsSystem, string Description, IEnumerable<string> PermissionClaims);
}
