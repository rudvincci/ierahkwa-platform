using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Security;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using RoleId = Mamey.Types.RoleId;

namespace Mamey.FWID.Identities.Infrastructure.EF;

internal sealed class IdentityInitializer
{
    private readonly IdentityDbContext _dbContext;
    private readonly ILogger<IdentityInitializer> _logger;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityRoleRepository _identityRoleRepository;
    private readonly IIdentityUnitOfWork _unitOfWork;
    private readonly ISecurityProvider _securityProvider;

    // Admin Identity ID (for application seed data)
    private static readonly IdentityId AdminIdentityId = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));

    // Deterministic Identity IDs for consistent seeding across all services (test data)
    private static readonly IdentityId IdentityAlphaId = new(Guid.Parse("11111111-1111-1111-1111-111111111111"));
    private static readonly IdentityId IdentityBetaId = new(Guid.Parse("22222222-2222-2222-2222-222222222222"));
    private static readonly IdentityId IdentityGammaId = new(Guid.Parse("33333333-3333-3333-3333-333333333333"));
    private static readonly IdentityId IdentityDeltaId = new(Guid.Parse("44444444-4444-4444-4444-444444444444"));
    private static readonly IdentityId IdentityEpsilonId = new(Guid.Parse("55555555-5555-5555-5555-555555555555"));
    private static readonly IdentityId IdentityZetaId = new(Guid.Parse("66666666-6666-6666-6666-666666666666"));
    private static readonly IdentityId IdentityEtaId = new(Guid.Parse("77777777-7777-7777-7777-777777777777"));
    private static readonly IdentityId IdentityThetaId = new(Guid.Parse("88888888-8888-8888-8888-888888888888"));
    private static readonly IdentityId IdentityIotaId = new(Guid.Parse("99999999-9999-9999-9999-999999999999"));
    private static readonly IdentityId IdentityKappaId = new(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    private static readonly IdentityId IdentityLambdaId = new(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
    private static readonly IdentityId IdentityMuId = new(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));

    public IdentityInitializer(
        IdentityDbContext dbContext,
        ILogger<IdentityInitializer> logger,
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IIdentityRoleRepository identityRoleRepository,
        IIdentityUnitOfWork unitOfWork,
        ISecurityProvider securityProvider)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _identityRoleRepository = identityRoleRepository ?? throw new ArgumentNullException(nameof(identityRoleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
    }

    public async Task InitAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogInformation("Starting identity system initialization...");

        // Ensure database exists and schema is up to date
#if DEBUG
        await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
        _logger.LogInformation("Database ensured (DEBUG mode).");
#else
        // In release mode, apply migrations
        await _dbContext.Database.MigrateAsync(cancellationToken);
        _logger.LogInformation("Database migrations applied (RELEASE mode).");
#endif

        // Check if data already exists
        var permissions = await _permissionRepository.BrowseAsync(cancellationToken);
        var roles = await _roleRepository.BrowseAsync(cancellationToken);
        var existingIdentities = await _dbContext.Identitys
            .AsNoTracking()
            .Select(i => i.Id.Value)
            .ToListAsync(cancellationToken);

        if (permissions.Any() || roles.Any() || existingIdentities.Any())
        {
            _logger.LogInformation("Identity data already exists (Permissions: {PermCount}, Roles: {RoleCount}, Identities: {IdentityCount}), skipping initialization.",
                permissions.Count, roles.Count, existingIdentities.Count);
            return;
        }

        try
        {
            // Use UnitOfWork.ExecuteAsync to handle transaction management
            await _unitOfWork.ExecuteAsync(async () =>
            {
                // Seed application data (permissions, roles, admin identity)
                await SeedApplicationDataAsync(cancellationToken);
                
                // Seed test data (test identities for development/testing)
                var testIdentityCount = await SeedTestDataAsync(cancellationToken);
                
                // Save all changes (UnitOfWork will commit the transaction)
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Identity system initialization completed successfully. Added application data and {Count} test identity(ies).", testIdentityCount);
            });
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            _logger.LogWarning("Duplicate key violation detected. Data may already exist. Skipping initialization.");
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize identity system.");
            throw;
        }
    }

    /// <summary>
    /// Seeds application data: permissions, roles, and at least one admin identity.
    /// This data is required for the application to function properly.
    /// </summary>
    private async Task SeedApplicationDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding application data (permissions, roles, admin identity)...");
        
        await AddPermissionsAsync(cancellationToken);
        await AddRolesAsync(cancellationToken);
        await AddAdminIdentityAsync(cancellationToken);
        
        _logger.LogInformation("Application data seeding completed.");
    }

    /// <summary>
    /// Seeds test data: test identities for development and testing purposes.
    /// </summary>
    private Task<int> SeedTestDataAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Seeding test data (test identities)...");
        
        var count = SeedTestIdentities(cancellationToken);
        
        _logger.LogInformation("Test data seeding completed. Added {Count} test identities.", count);
        
        return Task.FromResult(count);
    }

    private async Task AddPermissionsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding permissions...");

        var permissions = new List<Permission>
        {
            // ===== USER MANAGEMENT PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111111")), 
                "Create User", "Create new users in the system"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), 
                "Read User", "View user information and details"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")), 
                "Update User", "Modify user information and settings"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111114")), 
                "Delete User", "Delete users from the system"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")), 
                "Manage User Status", "Activate, deactivate, or suspend users"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111116")), 
                "Bulk User Operations", "Perform bulk operations on multiple users"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")), 
                "Export User Data", "Export user data for reporting or migration"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111118")), 
                "Import User Data", "Import user data from external sources"),
            new Permission(new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111119")), 
                "Audit User Actions", "View audit trail of user actions"),

            // ===== IDENTITY MANAGEMENT PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222221")), 
                "Create Identity", "Create new identities in the system"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), 
                "Read Identity", "View identity information and details"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), 
                "Update Identity", "Modify identity information and settings"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222224")), 
                "Delete Identity", "Delete identities from the system"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")), 
                "Manage Identity Status", "Activate, deactivate, or suspend identities"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")), 
                "Assign Identity Roles", "Assign or remove roles from identities"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222227")), 
                "View Identity History", "View complete history of identity changes"),
            new Permission(new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222228")), 
                "Merge Identities", "Merge duplicate or related identities"),

            // ===== ROLE MANAGEMENT PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333331")), 
                "Create Role", "Create new roles in the system"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), 
                "Read Role", "View role information and details"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333333")), 
                "Update Role", "Modify role information and settings"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333334")), 
                "Delete Role", "Delete roles from the system"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")), 
                "Assign Role", "Assign roles to identities or users"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333336")), 
                "Manage Role Permissions", "Add or remove permissions from roles"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333337")), 
                "Clone Role", "Create new roles based on existing ones"),
            new Permission(new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333338")), 
                "Audit Role Changes", "View audit trail of role modifications"),

            // ===== PERMISSION MANAGEMENT PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444441")), 
                "Create Permission", "Create new permissions in the system"),
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")), 
                "Read Permission", "View permission information and details"),
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444443")), 
                "Update Permission", "Modify permission information and settings"),
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444444")), 
                "Delete Permission", "Delete permissions from the system"),
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444445")), 
                "Manage Permission Groups", "Organize permissions into logical groups"),
            new Permission(new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444446")), 
                "Audit Permission Usage", "View how permissions are being used"),

            // ===== SESSION MANAGEMENT PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), 
                "View Sessions", "View active and historical user sessions"),
            new Permission(new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555552")), 
                "Manage Sessions", "Terminate, extend, or modify user sessions"),
            new Permission(new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555553")), 
                "Force Logout", "Force logout of specific users or all users"),
            new Permission(new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555554")), 
                "View Session Analytics", "View session statistics and analytics"),
            new Permission(new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555555")), 
                "Manage Session Policies", "Configure session timeout and security policies"),

            // ===== SYSTEM ADMINISTRATION PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666661")), 
                "System Admin", "Full system administration access"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666662")), 
                "View System Logs", "View system operational logs and trails"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666663")), 
                "Manage System Settings", "Modify system configuration and settings"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666664")), 
                "System Maintenance", "Perform system maintenance and updates"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666665")), 
                "Database Management", "Manage database operations and backups"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666666")), 
                "System Monitoring", "Monitor system health and performance"),
            new Permission(new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666667")), 
                "Emergency Access", "Emergency access to system during critical situations"),

            // ===== SECURITY PERMISSIONS =====
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")), 
                "Manage 2FA", "Manage two-factor authentication settings"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992")), 
                "Manage MFA", "Manage multi-factor authentication settings"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")), 
                "Reset Passwords", "Reset user passwords and credentials"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")), 
                "Unlock Accounts", "Unlock locked user accounts"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999995")), 
                "Manage Security Policies", "Configure security policies and rules"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), 
                "View Security Events", "View security events and alerts"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999997")), 
                "Manage API Keys", "Manage API keys and tokens"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998")), 
                "Audit Security", "Audit security-related activities"),
            new Permission(new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999999")), 
                "Manage Certificates", "Manage SSL certificates and digital certificates")
        };

        foreach (var permission in permissions)
        {
            await _permissionRepository.AddAsync(permission, cancellationToken);
        }
        
        _logger.LogInformation("Added {Count} permissions", permissions.Count);
    }

    private async Task AddRolesAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding roles...");

        var roles = new List<Role>
        {
            // SuperAdmin - Full system access (ALL permissions)
            CreateRoleWithPermissions(
                new RoleId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                "SuperAdmin",
                "Super Administrator with full system access and all permissions",
                GetAllPermissionIds()
            ),

            // System Administrator - System management without emergency access
            CreateRoleWithPermissions(
                new RoleId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                "SystemAdministrator",
                "System Administrator with comprehensive system management access",
                new[]
                {
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111111")), // Create User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")), // Update User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")), // Manage User Status
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")), // Export User Data
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111119")), // Audit User Actions
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222221")), // Create Identity
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Identity
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), // Update Identity
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")), // Manage Identity Status
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")), // Assign Identity Roles
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222227")), // View Identity History
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333331")), // Create Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333333")), // Update Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")), // Assign Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333336")), // Manage Role Permissions
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333337")), // Clone Role
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444441")), // Create Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")), // Read Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444443")), // Update Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444445")), // Manage Permission Groups
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), // View Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555552")), // Manage Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555553")), // Force Logout
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555554")), // View Session Analytics
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555555")), // Manage Session Policies
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666662")), // View System Logs
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666663")), // Manage System Settings
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666664")), // System Maintenance
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666665")), // Database Management
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666666")), // System Monitoring
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")), // Manage 2FA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992")), // Manage MFA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")), // Reset Passwords
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")), // Unlock Accounts
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999995")), // Manage Security Policies
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), // View Security Events
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999997")), // Manage API Keys
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998"))  // Audit Security
                }
            ),

            // Citizen - Basic citizen permissions
            CreateRoleWithPermissions(
                new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")),
                "Citizen",
                "Citizen with basic profile and service access permissions",
                new[]
                {
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Identity (own)
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), // Update Identity (own)
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")), // Manage 2FA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992"))  // Manage MFA
                }
            )
        };

        foreach (var role in roles)
        {
            await _roleRepository.AddAsync(role, cancellationToken);
        }
        
        _logger.LogInformation("Added {Count} roles", roles.Count);
    }

    private static Role CreateRoleWithPermissions(RoleId id, string name, string description, PermissionId[] permissionIds)
    {
        var role = new Role(id, name, description);
        foreach (var permissionId in permissionIds)
        {
            role.AddPermission(permissionId);
        }
        return role;
    }

    private static PermissionId[] GetAllPermissionIds()
    {
        return new[]
        {
            // User Management
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111111")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111114")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111116")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111118")),
            new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111119")),
            
            // Identity Management
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222221")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222224")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222227")),
            new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222228")),
            
            // Role Management
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333331")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333333")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333334")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333336")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333337")),
            new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333338")),
            
            // Permission Management
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444441")),
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")),
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444443")),
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444444")),
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444445")),
            new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444446")),
            
            // Session Management
            new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")),
            new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555552")),
            new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555553")),
            new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555554")),
            new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555555")),
            
            // System Administration
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666661")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666662")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666663")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666664")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666665")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666666")),
            new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666667")),
            
            // Security
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999995")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999997")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999999"))
        };
    }

    /// <summary>
    /// Creates the default admin identity with SuperAdmin role.
    /// </summary>
    private async Task AddAdminIdentityAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding admin identity...");

        // Check if admin identity already exists
        var existingAdmin = await _dbContext.Identitys
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == AdminIdentityId, cancellationToken);

        if (existingAdmin != null)
        {
            _logger.LogInformation("Admin identity already exists, skipping.");
            return;
        }

        // Create admin identity
        var adminIdentity = CreateIdentity(
            AdminIdentityId,
            new Name("System", "Administrator"),
            new PersonalDetails(new DateTime(2000, 1, 1), "System", "Other", null),
            new ContactInformation(
                new Email("admin@futurewampumid.com"),
                new Address("", "System Address", null, null, null, "System", "System", "00000", null, null, "US", "System"),
                new List<Phone>()
            ),
            CreateBiometricData(BiometricType.Fingerprint, "admin-fingerprint"),
            "system"
        );

        _dbContext.Identitys.Add(adminIdentity);

        // Assign SuperAdmin role to admin identity
        var superAdminRoleId = new RoleId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        var identityRole = new IdentityRole(AdminIdentityId, superAdminRoleId);
        await _identityRoleRepository.AddAsync(identityRole, cancellationToken);

        _logger.LogInformation("Admin identity created with SuperAdmin role.");
    }

    /// <summary>
    /// Seeds test identities for development and testing purposes.
    /// </summary>
    private int SeedTestIdentities(CancellationToken cancellationToken)
    {
        var identities = new List<Identity>();

        // Identity Alpha
        identities.Add(CreateIdentity(
            IdentityAlphaId,
            new Name("Ariana", "Lopez", "M."),
            new PersonalDetails(new DateTime(1990, 6, 15), "San Juan, PR", "Female", "Bear Clan"),
            new ContactInformation(
                new Email("ariana.lopez@example.com"),
                new Address("", "123 Calle del Sol", null, null, "Urb. Jardines", "San Juan", "PR", "00901", null, null, "US", "PR"),
                new List<Phone> { new Phone("1", "7875550101", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Fingerprint, "alpha-fingerprint"),
            "zone-001"
        ));

        // Identity Beta
        identities.Add(CreateIdentity(
            IdentityBetaId,
            new Name("Kwame", "Mensah"),
            new PersonalDetails(new DateTime(1985, 3, 22), "Accra, Ghana", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("kwame.mensah@example.com"),
                new Address("", "456 Oak Street", null, null, null, "Toronto", "ON", "", null, "M5H 2N2", "CA", "ON"),
                new List<Phone> { new Phone("1", "4165550202", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Facial, "beta-facial"),
            "zone-001"
        ));

        // Identity Gamma
        identities.Add(CreateIdentity(
            IdentityGammaId,
            new Name("Sakura", "Tanaka", "Yuki"),
            new PersonalDetails(new DateTime(1992, 9, 8), "Tokyo, Japan", "Female", "Eagle Clan"),
            new ContactInformation(
                new Email("sakura.tanaka@example.com"),
                new Address("", "789 Cherry Blossom Ave", null, null, null, "Tokyo", "Tokyo", "", null, "100-0001", "JP", "Tokyo"),
                new List<Phone> { new Phone("81", "3123456789", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Iris, "gamma-iris"),
            "zone-002"
        ));

        // Identity Delta
        identities.Add(CreateIdentity(
            IdentityDeltaId,
            new Name("Marcus", "Johnson", "David"),
            new PersonalDetails(new DateTime(1988, 12, 5), "Chicago, IL", "Male", "Turtle Clan"),
            new ContactInformation(
                new Email("marcus.johnson@example.com"),
                new Address("", "321 Lake Shore Drive", "Apt 4B", null, null, "Chicago", "IL", "60611", null, null, "US", "IL"),
                new List<Phone> { new Phone("1", "3125550303", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Fingerprint, "delta-fingerprint"),
            "zone-002"
        ));

        // Identity Epsilon
        identities.Add(CreateIdentity(
            IdentityEpsilonId,
            new Name("Isabella", "Rodriguez", "Maria"),
            new PersonalDetails(new DateTime(1995, 7, 20), "Madrid, Spain", "Female", "Bear Clan"),
            new ContactInformation(
                new Email("isabella.rodriguez@example.com"),
                new Address("", "654 Gran Via", null, null, null, "Madrid", "Madrid", "", null, "28013", "ES", "Madrid"),
                new List<Phone> { new Phone("34", "912345678", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Voice, "epsilon-voice"),
            "zone-003"
        ));

        // Identity Zeta
        identities.Add(CreateIdentity(
            IdentityZetaId,
            new Name("Ahmed", "Hassan", "Ali"),
            new PersonalDetails(new DateTime(1987, 4, 14), "Cairo, Egypt", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("ahmed.hassan@example.com"),
                new Address("", "987 Nile Street", null, null, null, "Cairo", "Cairo", "", null, "11511", "EG", "Cairo"),
                new List<Phone> { new Phone("20", "2123456789", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Facial, "zeta-facial"),
            "zone-001"
        ));

        // Identity Eta
        identities.Add(CreateIdentity(
            IdentityEtaId,
            new Name("Emma", "Anderson", "Rose"),
            new PersonalDetails(new DateTime(1993, 11, 30), "London, UK", "Female", "Eagle Clan"),
            new ContactInformation(
                new Email("emma.anderson@example.com"),
                new Address("", "147 Baker Street", null, null, null, "London", "England", "", null, "NW1 6XE", "GB", "England"),
                new List<Phone> { new Phone("44", "2071234567", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Fingerprint, "eta-fingerprint"),
            "zone-002"
        ));

        // Identity Theta
        identities.Add(CreateIdentity(
            IdentityThetaId,
            new Name("Chen", "Wei", "Ming"),
            new PersonalDetails(new DateTime(1991, 2, 18), "Beijing, China", "Male", "Turtle Clan"),
            new ContactInformation(
                new Email("chen.wei@example.com"),
                new Address("", "258 Wangfujing Street", null, null, null, "Beijing", "Beijing", "", null, "100006", "CN", "Beijing"),
                new List<Phone> { new Phone("86", "1012345678", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Iris, "theta-iris"),
            "zone-003"
        ));

        // Identity Iota
        identities.Add(CreateIdentity(
            IdentityIotaId,
            new Name("Sophia", "Martinez", "Isabella"),
            new PersonalDetails(new DateTime(1994, 8, 25), "Mexico City, Mexico", "Female", "Bear Clan"),
            new ContactInformation(
                new Email("sophia.martinez@example.com"),
                new Address("", "369 Reforma Avenue", null, null, null, "Mexico City", "CDMX", "", null, "06500", "MX", "CDMX"),
                new List<Phone> { new Phone("52", "5512345678", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Voice, "iota-voice"),
            "zone-001"
        ));

        // Identity Kappa
        identities.Add(CreateIdentity(
            IdentityKappaId,
            new Name("James", "O'Brien", "Patrick"),
            new PersonalDetails(new DateTime(1989, 1, 10), "Dublin, Ireland", "Male", "Wolf Clan"),
            new ContactInformation(
                new Email("james.obrien@example.com"),
                new Address("", "741 O'Connell Street", null, null, null, "Dublin", "Dublin", "", null, "D01 F5P2", "IE", "Dublin"),
                new List<Phone> { new Phone("353", "121234567", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Facial, "kappa-facial"),
            "zone-002"
        ));

        // Identity Lambda
        identities.Add(CreateIdentity(
            IdentityLambdaId,
            new Name("Priya", "Patel", "Anjali"),
            new PersonalDetails(new DateTime(1996, 5, 3), "Mumbai, India", "Female", "Eagle Clan"),
            new ContactInformation(
                new Email("priya.patel@example.com"),
                new Address("", "852 Marine Drive", null, null, null, "Mumbai", "Maharashtra", "", null, "400020", "IN", "Maharashtra"),
                new List<Phone> { new Phone("91", "2212345678", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Fingerprint, "lambda-fingerprint"),
            "zone-003"
        ));

        // Identity Mu
        identities.Add(CreateIdentity(
            IdentityMuId,
            new Name("Lucas", "Silva", "Fernando"),
            new PersonalDetails(new DateTime(1990, 10, 17), "São Paulo, Brazil", "Male", "Turtle Clan"),
            new ContactInformation(
                new Email("lucas.silva@example.com"),
                new Address("", "963 Paulista Avenue", null, null, null, "São Paulo", "SP", "", null, "01310-100", "BR", "SP"),
                new List<Phone> { new Phone("55", "1123456789", null, Phone.PhoneType.Mobile) }
            ),
            CreateBiometricData(BiometricType.Iris, "mu-iris"),
            "zone-001"
        ));

        _dbContext.Identitys.AddRange(identities);
        return identities.Count;
    }

    private Identity CreateIdentity(
        IdentityId id,
        Name name,
        PersonalDetails personalDetails,
        ContactInformation contactInformation,
        BiometricData biometricData,
        string? zone = null)
    {
        return new Identity(
            id,
            name,
            personalDetails,
            contactInformation,
            biometricData,
            zone
        );
    }

    private BiometricData CreateBiometricData(BiometricType type, string identifier)
    {
        // Generate deterministic biometric data based on identifier
        var template = Encoding.UTF8.GetBytes($"biometric-template-{identifier}");
        var hashBytes = _securityProvider.Hash(template);
        var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
        return new BiometricData(type, template, hash);
    }
}
