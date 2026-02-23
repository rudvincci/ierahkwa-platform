using System;
using System.Linq;
using System.Linq.Expressions;
using Mamey.Auth.Identity.Data;
using Mamey.Auth.Identity.Entities;
using Mamey.Auth.Identity.Providers;
using Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;
using Mamey.Persistence.SQL;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.ApplicationName.Modules.Identity.Core.EF
{
    internal sealed class ApplicationIdentityDbContext : MameyIdentityDbContext
    {
        public DbSet<InboxMessage> Inbox => Set<InboxMessage>();
        public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
        public new DbSet<ApplicationUser> Users => base.Users;
        public new DbSet<ApplicationRole> Roles => base.Roles;
        public new DbSet<ApplicationUserRole> UserRoles => base.UserRoles;
        public new DbSet<ApplicationRoleClaim> RoleClaims => base.RoleClaims;
        public new DbSet<ApplicationUserLogin> UserLogins => base.UserLogins;
        public new DbSet<ApplicationUserClaim> UserClaims => base.UserClaims;
        public new DbSet<ApplicationUserToken> UserTokens => base.UserTokens;
        public DbSet<Permission> Permissions => base.Permissions;
        public DbSet<RoleHierarchy> RoleHierarchies => base.RoleHierarchies;
        public DbSet<UserPreference> UserPreferences => base.UserPreferences;
        public DbSet<UserLoginAttempt> LoginAttempts => base.LoginAttempts;

        public DbSet<AuditLog> AuditLogs => base.AuditLogs;
        public DbSet<MfaCode> MfaCodes => base.MfaCodes;

        public ApplicationIdentityDbContext(
            DbContextOptions<MameyIdentityDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<InboxMessage>().ToTable("inbox", "identity");
            mb.Entity<OutboxMessage>().ToTable("outbox", "identity");

            mb.ApplyConfigurationsFromAssembly(typeof(ApplicationIdentityDbContext).Assembly);
            mb.Ignore<UserId>();
            mb.Ignore<RoleId>();
            // mb.Ignore<Name>();
            mb.Ignore<Address>();
            mb.Ignore<Phone>();
            mb.Ignore<Email>();
            // if (_tenant.TryGetTenantId(out var tenantId) && tenantId.Value != Guid.Empty)
            // {
            //     foreach (var entityType in mb.Model.GetEntityTypes()
            //         .Where(t => typeof(ITenantScoped).IsAssignableFrom(t.ClrType)))
            //     {
            //         var param = Expression.Parameter(entityType.ClrType, "e");
            //         var body = Expression.Equal(
            //             Expression.Property(
            //                 Expression.Property(
            //                     Expression.Convert(param, typeof(ITenantScoped)),
            //                     nameof(ITenantScoped.TenantId)),
            //                 nameof(TenantId.Value)),
            //             Expression.Constant(tenantId.Value));
            //
            //         entityType.SetQueryFilter(Expression.Lambda(body, param));
            //     }
            // }
            mb.UseSnakeCaseNamingConvention();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}