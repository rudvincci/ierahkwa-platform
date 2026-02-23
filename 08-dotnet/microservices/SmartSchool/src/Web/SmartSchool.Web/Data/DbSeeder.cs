using Common.Domain.Entities;
using Common.Persistence;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Persistence;

namespace SmartSchool.Web.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        var commonContext = services.GetRequiredService<CommonDbContext>();
        var userContext = services.GetRequiredService<UserManagementDbContext>();
        
        await SeedTenantsAsync(commonContext);
        await SeedUsersAsync(userContext);
    }

    private static async Task SeedTenantsAsync(CommonDbContext context)
    {
        if (await context.Tenants.AnyAsync())
            return;

        var defaultTenant = new Tenant
        {
            Name = "Default School",
            Code = "DEFAULT",
            Email = "admin@smartschool.com",
            Currency = "USD",
            IsActive = true,
            DefaultLanguage = "en",
            CreatedAt = DateTime.UtcNow
        };

        await context.Tenants.AddAsync(defaultTenant);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(UserManagementDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        // Create admin user
        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@smartschool.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd"),
            FirstName = "System",
            LastName = "Administrator",
            IsActive = true,
            EmailConfirmed = true,
            TenantId = null, // Super admin has no tenant
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        // Assign admin role
        var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoles.Admin);
        if (adminRole != null)
        {
            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                CreatedAt = DateTime.UtcNow
            };
            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }
    }
}
