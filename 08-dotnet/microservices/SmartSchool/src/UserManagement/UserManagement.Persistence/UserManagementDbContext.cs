using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;

namespace UserManagement.Persistence;

public class UserManagementDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }

    public UserManagementDbContext(
        DbContextOptions<UserManagementDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<RolePolicy> RolePolicies => Set<RolePolicy>();
    public DbSet<AuthenticationSettings> AuthenticationSettings => Set<AuthenticationSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("Policies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.Controller).HasMaxLength(100);
            entity.Property(e => e.Action).HasMaxLength(100);
        });

        modelBuilder.Entity<RolePolicy>(entity =>
        {
            entity.ToTable("RolePolicies");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePolicies)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Policy)
                .WithMany(p => p.RolePolicies)
                .HasForeignKey(e => e.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.RoleId, e.PolicyId }).IsUnique();
        });

        modelBuilder.Entity<AuthenticationSettings>(entity =>
        {
            entity.ToTable("AuthenticationSettings");
            entity.HasKey(e => e.Id);
        });

        // Seed default roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = DefaultRoles.Admin, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 2, Name = DefaultRoles.SchoolAdmin, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 3, Name = DefaultRoles.Accountant, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 4, Name = DefaultRoles.Teacher, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 5, Name = DefaultRoles.Student, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 6, Name = DefaultRoles.Parent, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 7, Name = DefaultRoles.Receptionist, IsSystemRole = true, CreatedAt = DateTime.UtcNow },
            new Role { Id = 8, Name = DefaultRoles.Librarian, IsSystemRole = true, CreatedAt = DateTime.UtcNow }
        );
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.UserName;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService?.UserName;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
