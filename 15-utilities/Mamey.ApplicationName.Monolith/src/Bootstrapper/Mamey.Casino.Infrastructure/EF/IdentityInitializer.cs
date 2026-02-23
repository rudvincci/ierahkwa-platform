using Mamey.Auth.Identity.Abstractions;
using Mamey.Auth.Identity.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Types;

namespace Mamey.Casino.Infrastructure.EF
{
    /// <summary>
    /// Seeds initial Identity data: roles, permissions, and the default admin user.
    /// </summary>
    internal sealed class IdentityInitializer : IInitializer
    {
        private readonly CasinoDbContext _dbContext;
        private readonly ILogger<IdentityInitializer> _logger;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly RolePermissions _rolePermissions;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityInitializer"/> class.
        /// </summary>
        /// <param name="dbContext">The EF Core database context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="passwordHasher">The password hasher for <see cref="ApplicationUser"/>.</param>
        public IdentityInitializer(
            CasinoDbContext dbContext,
            ILogger<IdentityInitializer> logger,
            IPasswordHasher<ApplicationUser> passwordHasher, RoleManager<ApplicationRole> roleManager, RolePermissions rolePermissions, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _roleManager = roleManager;
            _rolePermissions = rolePermissions;
            _userManager = userManager;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {

            // 1) Always seed all roles & permissions
            await SeedRolesAndPermissionsAsync(_roleManager, _rolePermissions);

            // 2) Always ensure the admin user exists with its role & claims
            await AddAdminUserAsync(_userManager);

            // Persist any changes
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Users, roles, and permissions initialized.");
        }

        /// <summary>
        /// Ensures that every role defined in <see cref="RolePermissions.Permissions"/> exists,
        /// with its associated permission enum and a permission claim.
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <param name="rolePermissions">Mapping of permission‐type → (role name → permission value).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task SeedRolesAndPermissionsAsync(
            RoleManager<ApplicationRole> roleManager,
            RolePermissions rolePermissions)
        {

            foreach (var (permissionType, roles) in rolePermissions.Permissions)
            {
                foreach (var (roleName, rawValue) in roles)
                {
                    if (await roleManager.RoleExistsAsync(roleName))
                        continue;

                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} Role"
                    };

                    // Set the enum property (e.g. BankPermission or MemberPermission)
                    var prop = typeof(ApplicationRole).GetProperty(permissionType.Name);
                    if (prop != null)
                    {
                        var enumValue = Enum.ToObject(permissionType, (long)rawValue);
                        prop.SetValue(role, enumValue);
                    }

                    var createResult = await roleManager.CreateAsync(role);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                    }

                    // Add a "Permission" claim to the role
                    var claim = new System.Security.Claims.Claim("Permission", rawValue.ToString());
                    var claimResult = await roleManager.AddClaimAsync(role, claim);
                    if (!claimResult.Succeeded)
                    {
                        var errors = string.Join(", ", claimResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to add permission claim to role '{roleName}': {errors}");
                    }
                }
            }
        }

        /// <summary>
        /// Ensures the default admin user exists, is in the "Admin" role,
        /// and has the appropriate claims (including <see cref="System.Security.Claims.ClaimTypes.Role"/>).
        /// </summary>
        /// <param name="serviceProvider">Application service provider.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
   
            const string adminEmail    = "admin@example.com";
            const string adminPassword = "Admin@123";
            const string adminRole     = "Admin";

            // If the admin already exists, nothing to do
            var existing = await userManager.FindByEmailAsync(adminEmail);
            if (existing != null)
                return;

            // Create the admin user
            var name = new Name("System", "Administrator");
            var admin = new ApplicationUser(new Guid("00000000-0000-0000-0000-000000000001"))
            {
                UserName       = "admin",
                Email          = adminEmail,
                EmailConfirmed = true,
                Name           = name,
                FullName       = name.FullName
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user: {errors}");
            }

            // Assign the Admin role
            var roleResult = await userManager.AddToRoleAsync(admin, adminRole);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role '{adminRole}' to admin: {errors}");
            }

            // Add essential claims
            var claims = new[]
            {
                // Display name claim
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Name, admin.FullName),

                // Standard role claim for [Authorize(Roles="Admin")]
                new System.Security.Claims.Claim(
                    System.Security.Claims.ClaimTypes.Role, adminRole),

                // Full-permission marker
                new System.Security.Claims.Claim("Permission", "All")
            };

            foreach (var claim in claims)
            {
                var claimResult = await userManager.AddClaimAsync(admin, claim);
                if (!claimResult.Succeeded)
                {
                    var errors = string.Join(", ", claimResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to add claim '{claim.Type}' to admin: {errors}");
                }
            }
        }
    }
}

