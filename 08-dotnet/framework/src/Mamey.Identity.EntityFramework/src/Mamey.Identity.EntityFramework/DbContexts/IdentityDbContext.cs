using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mamey.Identity.EntityFramework.Entities;
using Mamey.Identity.EntityFramework.Configurations;

namespace Mamey.Identity.EntityFramework.DbContexts;

public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());

        // Configure table names
        builder.Entity<ApplicationUserRole>().ToTable("UserRoles");
        builder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
        builder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
        builder.Entity<ApplicationUserToken>().ToTable("UserTokens");
        builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");

        // Configure relationships
        builder.Entity<ApplicationUserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserClaim>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserClaims)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserLogin>()
            .HasOne(ul => ul.User)
            .WithMany(u => u.UserLogins)
            .HasForeignKey(ul => ul.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserToken>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.UserTokens)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationRoleClaim>()
            .HasOne(rc => rc.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

