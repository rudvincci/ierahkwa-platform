using System.Security.Cryptography;
using System.Text;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Security;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Mamey.Government.Identity.Infrastructure.EF;

internal sealed class UserInitializer
{
    private readonly UserDbContext _dbContext;
    private readonly ILogger<UserInitializer> _logger;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICredentialRepository _credentialRepository;
    private readonly IHasher _hasher;

    public UserInitializer(
        UserDbContext dbContext, 
        ILogger<UserInitializer> logger,
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        ISubjectRepository subjectRepository,
        IUserRepository userRepository,
        ICredentialRepository credentialRepository, IHasher hasher)
    {
        _dbContext = dbContext;
        _logger = logger;
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _subjectRepository = subjectRepository;
        _userRepository = userRepository;
        _credentialRepository = credentialRepository;
        _hasher = hasher;
    }

    public async Task InitAsync()
    {
        #if DEBUG
        await _dbContext.Database.EnsureCreatedAsync();
        #else
        // Ensure database exists and migrations are applied
        await _dbContext.Database.MigrateAsync();
        #endif
        _logger.LogInformation("Starting identity system initialization...");

        // Check if data already exists - use repositories for consistency
        var permissions = await _permissionRepository.BrowseAsync();
        var roles = await _roleRepository.BrowseAsync();
        var subjects = await _subjectRepository.BrowseAsync();
        var users = await _userRepository.BrowseAsync();
        var credentials = await _credentialRepository.BrowseAsync();

        if (permissions.Any() || roles.Any() || subjects.Any() || users.Any() || credentials.Any())
        {
            _logger.LogInformation("Identity data already exists, skipping initialization.");
            return;
        }

        try
        {
            // Initialize in order: Permissions -> Roles -> Subjects -> Users -> Credentials
            // Using Composite repositories ensures data is written to Postgres, Redis, and Mongo
            await AddPermissionsAsync();
            await AddRolesAsync();
            await AddSubjectsAsync();
            await AddUsersAsync();
            await AddCredentialsAsync();
            
            _logger.LogInformation("Identity system initialization completed successfully.");
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

    private async Task AddPermissionsAsync()
    {
        _logger.LogInformation("Adding permissions...");

        var permissions = new List<Permission>
        {
            // ===== USER MANAGEMENT PERMISSIONS =====
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                "Create User", "Create new users in the system", "users", "create"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111112"), 
                "Read User", "View user information and details", "users", "read"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111113"), 
                "Update User", "Modify user information and settings", "users", "update"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111114"), 
                "Delete User", "Delete users from the system", "users", "delete"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111115"), 
                "Manage User Status", "Activate, deactivate, or suspend users", "users", "manage_status"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111116"), 
                "Bulk User Operations", "Perform bulk operations on multiple users", "users", "bulk_operations"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111117"), 
                "Export User Data", "Export user data for reporting or migration", "users", "export"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111118"), 
                "Import User Data", "Import user data from external sources", "users", "import"),
            Permission.Create(Guid.Parse("11111111-1111-1111-1111-111111111119"), 
                "Audit User Actions", "View audit trail of user actions", "users", "audit"),

            // ===== SUBJECT MANAGEMENT PERMISSIONS =====
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222221"), 
                "Create Subject", "Create new subjects in the identity system", "subjects", "create"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222222"), 
                "Read Subject", "View subject information and details", "subjects", "read"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222223"), 
                "Update Subject", "Modify subject information and settings", "subjects", "update"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222224"), 
                "Delete Subject", "Delete subjects from the system", "subjects", "delete"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222225"), 
                "Manage Subject Status", "Activate, deactivate, or suspend subjects", "subjects", "manage_status"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222226"), 
                "Assign Subject Roles", "Assign or remove roles from subjects", "subjects", "assign_roles"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222227"), 
                "View Subject History", "View complete history of subject changes", "subjects", "view_history"),
            Permission.Create(Guid.Parse("22222222-2222-2222-2222-222222222228"), 
                "Merge Subjects", "Merge duplicate or related subjects", "subjects", "merge"),

            // ===== ROLE MANAGEMENT PERMISSIONS =====
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333331"), 
                "Create Role", "Create new roles in the system", "roles", "create"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333332"), 
                "Read Role", "View role information and details", "roles", "read"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333333"), 
                "Update Role", "Modify role information and settings", "roles", "update"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333334"), 
                "Delete Role", "Delete roles from the system", "roles", "delete"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333335"), 
                "Assign Role", "Assign roles to subjects or users", "roles", "assign"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333336"), 
                "Manage Role Permissions", "Add or remove permissions from roles", "roles", "manage_permissions"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333337"), 
                "Clone Role", "Create new roles based on existing ones", "roles", "clone"),
            Permission.Create(Guid.Parse("33333333-3333-3333-3333-333333333338"), 
                "Audit Role Changes", "View audit trail of role modifications", "roles", "audit"),

            // ===== PERMISSION MANAGEMENT PERMISSIONS =====
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444441"), 
                "Create Permission", "Create new permissions in the system", "permissions", "create"),
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444442"), 
                "Read Permission", "View permission information and details", "permissions", "read"),
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444443"), 
                "Update Permission", "Modify permission information and settings", "permissions", "update"),
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                "Delete Permission", "Delete permissions from the system", "permissions", "delete"),
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444445"), 
                "Manage Permission Groups", "Organize permissions into logical groups", "permissions", "manage_groups"),
            Permission.Create(Guid.Parse("44444444-4444-4444-4444-444444444446"), 
                "Audit Permission Usage", "View how permissions are being used", "permissions", "audit_usage"),

            // ===== SESSION MANAGEMENT PERMISSIONS =====
            Permission.Create(Guid.Parse("55555555-5555-5555-5555-555555555551"), 
                "View Sessions", "View active and historical user sessions", "sessions", "read"),
            Permission.Create(Guid.Parse("55555555-5555-5555-5555-555555555552"), 
                "Manage Sessions", "Terminate, extend, or modify user sessions", "sessions", "manage"),
            Permission.Create(Guid.Parse("55555555-5555-5555-5555-555555555553"), 
                "Force Logout", "Force logout of specific users or all users", "sessions", "force_logout"),
            Permission.Create(Guid.Parse("55555555-5555-5555-5555-555555555554"), 
                "View Session Analytics", "View session statistics and analytics", "sessions", "analytics"),
            Permission.Create(Guid.Parse("55555555-5555-5555-5555-555555555555"), 
                "Manage Session Policies", "Configure session timeout and security policies", "sessions", "manage_policies"),

            // ===== SYSTEM ADMINISTRATION PERMISSIONS =====
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666661"), 
                "System Admin", "Full system administration access", "system", "admin"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666662"), 
                "View System Logs", "View system operational logs and trails", "logs", "read"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666663"), 
                "Manage System Settings", "Modify system configuration and settings", "settings", "manage"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666664"), 
                "System Maintenance", "Perform system maintenance and updates", "system", "maintenance"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666665"), 
                "Database Management", "Manage database operations and backups", "database", "manage"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666666"), 
                "System Monitoring", "Monitor system health and performance", "system", "monitor"),
            Permission.Create(Guid.Parse("66666666-6666-6666-6666-666666666667"), 
                "Emergency Access", "Emergency access to system during critical situations", "system", "emergency"),

            // ===== CITIZEN SERVICES PERMISSIONS =====
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777771"), 
                "View Own Profile", "View own profile information", "profile", "read_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777772"), 
                "Update Own Profile", "Update own profile information", "profile", "update_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777773"), 
                "Apply for Services", "Apply for government services and benefits", "services", "apply"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777774"), 
                "View Own Applications", "View own service applications and status", "applications", "read_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777775"), 
                "Update Own Applications", "Update own pending applications", "applications", "update_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777776"), 
                "View Own Documents", "View own uploaded documents and certificates", "documents", "read_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777777"), 
                "Upload Documents", "Upload required documents for applications", "documents", "upload"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777778"), 
                "Schedule Appointments", "Schedule appointments with government offices", "appointments", "schedule"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-777777777779"), 
                "View Own Appointments", "View own scheduled appointments", "appointments", "read_own"),
            Permission.Create(Guid.Parse("77777777-7777-7777-7777-77777777777a"), 
                "Cancel Own Appointments", "Cancel own scheduled appointments", "appointments", "cancel_own"),

            // ===== STAFF SERVICES PERMISSIONS =====
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888881"), 
                "Process Applications", "Process citizen applications and requests", "applications", "process"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888882"), 
                "View All Applications", "View all citizen applications in the system", "applications", "read_all"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888883"), 
                "Generate Reports", "Generate system reports and analytics", "reports", "generate"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888884"), 
                "View Reports", "View generated reports and dashboards", "reports", "read"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888885"), 
                "Manage Appointments", "Manage citizen appointments and scheduling", "appointments", "manage"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888886"), 
                "Verify Documents", "Verify and validate citizen documents", "documents", "verify"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888887"), 
                "Process Payments", "Process payments for government services", "payments", "process_payment"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888888"), 
                "Issue Certificates", "Issue official certificates and documents", "certificates", "issue"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-888888888889"), 
                "Manage Workflows", "Manage application workflows and processes", "workflows", "manage"),
            Permission.Create(Guid.Parse("88888888-8888-8888-8888-88888888888a"), 
                "Escalate Issues", "Escalate complex issues to supervisors", "issues", "escalate"),

            // ===== SECURITY PERMISSIONS =====
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999991"), 
                "Manage 2FA", "Manage two-factor authentication settings", "security", "manage_2fa"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999992"), 
                "Manage MFA", "Manage multi-factor authentication settings", "security", "manage_mfa"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999993"), 
                "Reset Passwords", "Reset user passwords and credentials", "security", "reset_passwords"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999994"), 
                "Unlock Accounts", "Unlock locked user accounts", "security", "unlock_accounts"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999995"), 
                "Manage Security Policies", "Configure security policies and rules", "security", "manage_policies"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999996"), 
                "View Security Events", "View security events and alerts", "security", "view_events"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999997"), 
                "Manage API Keys", "Manage API keys and tokens", "security", "manage_api_keys"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999998"), 
                "Audit Security", "Audit security-related activities", "security", "audit"),
            Permission.Create(Guid.Parse("99999999-9999-9999-9999-999999999999"), 
                "Manage Certificates", "Manage SSL certificates and digital certificates", "security", "manage_certificates"),

            // ===== GOVERNMENT SERVICES PERMISSIONS =====
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01"), 
                "Citizenship Services", "Access citizenship-related services", "citizenship", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02"), 
                "Passport Services", "Access passport application and renewal services", "passport", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03"), 
                "ID Card Services", "Access national ID card services", "id_card", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04"), 
                "Visa Services", "Access visa application and processing services", "visa", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05"), 
                "Birth Certificate Services", "Access birth certificate services", "birth_certificate", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06"), 
                "Marriage Certificate Services", "Access marriage certificate services", "marriage_certificate", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07"), 
                "Death Certificate Services", "Access death certificate services", "death_certificate", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08"), 
                "Tax Services", "Access tax-related services and filing", "tax", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09"), 
                "Social Security Services", "Access social security and benefits services", "social_security", "access"),
            Permission.Create(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10"), 
                "Healthcare Services", "Access healthcare and medical services", "healthcare", "access"),

            // ===== FINANCIAL PERMISSIONS =====
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01"), 
                "Process Government Payments", "Process government service payments", "payments", "process"),
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02"), 
                "Refund Payments", "Process payment refunds", "payments", "refund"),
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03"), 
                "View Payment History", "View payment transaction history", "payments", "view_history"),
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04"), 
                "Manage Payment Methods", "Manage accepted payment methods", "payments", "manage_methods"),
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05"), 
                "Generate Invoices", "Generate invoices for government services", "payments", "generate_invoices"),
            Permission.Create(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06"), 
                "View Financial Reports", "View financial reports and analytics", "payments", "view_reports"),

            // ===== COMMUNICATION PERMISSIONS =====
            Permission.Create(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01"), 
                "Send Notifications", "Send notifications to users", "notifications", "send"),
            Permission.Create(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02"), 
                "Send Emails", "Send emails to users and citizens", "email", "send"),
            Permission.Create(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03"), 
                "Send SMS", "Send SMS messages to users", "sms", "send"),
            Permission.Create(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04"), 
                "Manage Templates", "Manage notification and communication templates", "templates", "manage"),
            Permission.Create(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05"), 
                "View Communication Logs", "View logs of sent communications", "communications", "view_logs"),

            // ===== AUDIT AND COMPLIANCE PERMISSIONS =====
            Permission.Create(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd01"), 
                "View Audit Logs", "View system audit logs and trails", "audit", "view_logs"),
            Permission.Create(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd02"), 
                "Export Audit Data", "Export audit data for compliance reporting", "audit", "export"),
            Permission.Create(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd03"), 
                "Compliance Reporting", "Generate compliance reports", "compliance", "report"),
            Permission.Create(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd04"), 
                "Data Retention Management", "Manage data retention policies", "compliance", "manage_retention"),
            Permission.Create(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd05"), 
                "Privacy Controls", "Manage privacy and data protection controls", "privacy", "manage"),

            // ===== INTEGRATION PERMISSIONS =====
            Permission.Create(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee01"), 
                "API Access", "Access to system APIs", "api", "access"),
            Permission.Create(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee02"), 
                "External System Integration", "Integrate with external government systems", "integration", "external"),
            Permission.Create(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03"), 
                "Data Synchronization", "Synchronize data with external systems", "integration", "sync"),
            Permission.Create(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04"), 
                "Webhook Management", "Manage webhooks for system events", "integration", "webhooks"),

            // ===== ANALYTICS AND REPORTING PERMISSIONS =====
            Permission.Create(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01"), 
                "View Analytics Dashboard", "View system analytics and dashboards", "analytics", "view_dashboard"),
            Permission.Create(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02"), 
                "Generate Custom Reports", "Generate custom reports and queries", "reports", "custom"),
            Permission.Create(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff03"), 
                "Export Data", "Export data for analysis", "data", "export"),
            Permission.Create(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04"), 
                "View Usage Statistics", "View system usage statistics", "analytics", "usage_stats"),
            Permission.Create(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff05"), 
                "Performance Monitoring", "Monitor system performance metrics", "analytics", "performance"),

            // ===== EMERGENCY AND DISASTER RECOVERY PERMISSIONS =====
            Permission.Create(Guid.Parse("00000000-0000-0000-0000-000000000001"), 
                "Emergency System Access", "Emergency access during critical situations", "emergency", "access"),
            Permission.Create(Guid.Parse("00000000-0000-0000-0000-000000000002"), 
                "Disaster Recovery", "Access disaster recovery procedures", "disaster", "recovery"),
            Permission.Create(Guid.Parse("00000000-0000-0000-0000-000000000003"), 
                "System Shutdown", "Emergency system shutdown capabilities", "emergency", "shutdown"),
            Permission.Create(Guid.Parse("00000000-0000-0000-0000-000000000004"), 
                "Data Backup", "Manage system data backups", "backup", "manage"),
            Permission.Create(Guid.Parse("00000000-0000-0000-0000-000000000005"), 
                "System Restore", "Restore system from backups", "backup", "restore")
        };

        foreach (var permission in permissions)
        {
            await _permissionRepository.AddAsync(permission);
        }
        
        _logger.LogInformation("Added {Count} permissions", permissions.Count);
    }

    private async Task AddRolesAsync()
    {
        _logger.LogInformation("Adding roles...");

        var roles = new List<Role>
        {
            // SuperAdmin - Full system access (ALL permissions)
            new Role(
                new RoleId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                "SuperAdmin",
                "Super Administrator with full system access and all permissions",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: GetAllPermissionIds(),
                status: RoleStatus.Active
            ),

            // System Administrator - System management without emergency access
            new Role(
                new RoleId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                "SystemAdministrator",
                "System Administrator with comprehensive system management access",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // User Management
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111111")), // Create User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")), // Update User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")), // Manage User Status
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")), // Export User Data
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111119")), // Audit User Actions
                    
                    // Subject Management
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222221")), // Create Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), // Update Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")), // Manage Subject Status
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")), // Assign Subject Roles
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222227")), // View Subject History
                    
                    // Role Management
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333331")), // Create Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333333")), // Update Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")), // Assign Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333336")), // Manage Role Permissions
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333337")), // Clone Role
                    
                    // Permission Management
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444441")), // Create Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")), // Read Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444443")), // Update Permission
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444445")), // Manage Permission Groups
                    
                    // Session Management
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), // View Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555552")), // Manage Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555553")), // Force Logout
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555554")), // View Session Analytics
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555555")), // Manage Session Policies
                    
                    // System Administration
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666662")), // View System Logs
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666663")), // Manage System Settings
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666664")), // System Maintenance
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666665")), // Database Management
                    new PermissionId(Guid.Parse("66666666-6666-6666-6666-666666666666")), // System Monitoring
                    
                    // Security
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")), // Manage 2FA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992")), // Manage MFA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")), // Reset Passwords
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")), // Unlock Accounts
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999995")), // Manage Security Policies
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), // View Security Events
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999997")), // Manage API Keys
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998")), // Audit Security
                    
                    // Analytics and Reporting
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")), // View Analytics Dashboard
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02")), // Generate Custom Reports
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff03")), // Export Data
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04")), // View Usage Statistics
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff05"))  // Performance Monitoring
                }
            ),

            // Government Administrator - Administrative access for government operations
            new Role(
                new RoleId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")),
                "GovernmentAdministrator",
                "Government Administrator with comprehensive operational access",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // User Management (limited)
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")), // Update User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")), // Manage User Status
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")), // Export User Data
                    
                    // Subject Management
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222221")), // Create Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), // Update Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")), // Manage Subject Status
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")), // Assign Subject Roles
                    
                    // Role Management (limited)
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")), // Assign Role
                    
                    // Staff Services
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888881")), // Process Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888882")), // View All Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888883")), // Generate Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888884")), // View Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888885")), // Manage Appointments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888886")), // Verify Documents
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888887")), // Process Payments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888888")), // Issue Certificates
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888889")), // Manage Workflows
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-88888888888a")), // Escalate Issues
                    
                    // Government Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01")), // Citizenship Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02")), // Passport Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03")), // ID Card Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04")), // Visa Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05")), // Birth Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06")), // Marriage Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07")), // Death Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08")), // Tax Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09")), // Social Security Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10")), // Healthcare Services
                    
                    // Financial
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01")), // Process Payments
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02")), // Refund Payments
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03")), // View Payment History
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05")), // Generate Invoices
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06")), // View Financial Reports
                    
                    // Communication
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01")), // Send Notifications
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02")), // Send Emails
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03")), // Send SMS
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04")), // Manage Templates
                    
                    // Security (limited)
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")), // Reset Passwords
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")), // Unlock Accounts
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), // View Security Events
                    
                    // Analytics
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")), // View Analytics Dashboard
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02")), // Generate Custom Reports
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04"))  // View Usage Statistics
                }
            ),

            // Staff - Government staff with service processing permissions
            new Role(
                new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")),
                "Staff",
                "Government staff member with service processing access",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // Basic read permissions
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Subject
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")), // Read Permission
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), // View Sessions
                    
                    // Staff Services
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888881")), // Process Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888882")), // View All Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888883")), // Generate Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888884")), // View Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888885")), // Manage Appointments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888886")), // Verify Documents
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888887")), // Process Payments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888888")), // Issue Certificates
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-88888888888a")), // Escalate Issues
                    
                    // Government Services (limited)
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01")), // Citizenship Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02")), // Passport Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03")), // ID Card Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05")), // Birth Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06")), // Marriage Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07")), // Death Certificate Services
                    
                    // Financial (limited)
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01")), // Process Payments
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03")), // View Payment History
                    
                    // Communication
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01")), // Send Notifications
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02")), // Send Emails
                    
                    // Analytics (limited)
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")), // View Analytics Dashboard
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04"))  // View Usage Statistics
                }
            ),

            // Manager - Staff with additional management permissions
            new Role(
                new RoleId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")),
                "Manager",
                "Staff manager with additional oversight and management permissions",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // User Management (limited)
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111113")), // Update User
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111115")), // Manage User Status
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111117")), // Export User Data
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111119")), // Audit User Actions
                    
                    // Subject Management
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222223")), // Update Subject
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222225")), // Manage Subject Status
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222226")), // Assign Subject Roles
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222227")), // View Subject History
                    
                    // Role Management (limited)
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333335")), // Assign Role
                    
                    // Session Management
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), // View Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555552")), // Manage Sessions
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555553")), // Force Logout
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555554")), // View Session Analytics
                    
                    // Staff Services (all)
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888881")), // Process Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888882")), // View All Applications
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888883")), // Generate Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888884")), // View Reports
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888885")), // Manage Appointments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888886")), // Verify Documents
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888887")), // Process Payments
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888888")), // Issue Certificates
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888889")), // Manage Workflows
                    new PermissionId(Guid.Parse("88888888-8888-8888-8888-88888888888a")), // Escalate Issues
                    
                    // Government Services (all)
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01")), // Citizenship Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02")), // Passport Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03")), // ID Card Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04")), // Visa Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05")), // Birth Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06")), // Marriage Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07")), // Death Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08")), // Tax Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09")), // Social Security Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10")), // Healthcare Services
                    
                    // Financial (all)
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01")), // Process Payments
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02")), // Refund Payments
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03")), // View Payment History
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04")), // Manage Payment Methods
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05")), // Generate Invoices
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06")), // View Financial Reports
                    
                    // Communication (all)
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01")), // Send Notifications
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02")), // Send Emails
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03")), // Send SMS
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04")), // Manage Templates
                    new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05")), // View Communication Logs
                    
                    // Security (limited)
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")), // Reset Passwords
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")), // Unlock Accounts
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), // View Security Events
                    
                    // Analytics (all)
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")), // View Analytics Dashboard
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02")), // Generate Custom Reports
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff03")), // Export Data
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04")), // View Usage Statistics
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff05"))  // Performance Monitoring
                }
            ),

            // Citizen - Basic citizen permissions
            new Role(
                new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")),
                "Citizen",
                "Citizen with basic profile and service access permissions",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // Citizen Services
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777771")), // View Own Profile
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777772")), // Update Own Profile
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777773")), // Apply for Services
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777774")), // View Own Applications
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777775")), // Update Own Applications
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777776")), // View Own Documents
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777777")), // Upload Documents
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777778")), // Schedule Appointments
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777779")), // View Own Appointments
                    new PermissionId(Guid.Parse("77777777-7777-7777-7777-77777777777a")), // Cancel Own Appointments
                    
                    // Government Services (limited)
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01")), // Citizenship Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02")), // Passport Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03")), // ID Card Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05")), // Birth Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06")), // Marriage Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07")), // Death Certificate Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08")), // Tax Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09")), // Social Security Services
                    new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10")), // Healthcare Services
                    
                    // Financial (limited)
                    new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03")), // View Payment History
                    
                    // Security (own account only)
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")), // Manage 2FA
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992"))  // Manage MFA
                }
            ),

            // Auditor - Read-only access for compliance and auditing
            new Role(
                new RoleId(Guid.Parse("11111111-1111-1111-1111-111111111111")),
                "Auditor",
                "Auditor with read-only access for compliance and auditing purposes",
                DateTime.UtcNow,
                modifiedAt: null,
                permissions: new[]
                {
                    // Read-only access to all entities
                    new PermissionId(Guid.Parse("11111111-1111-1111-1111-111111111112")), // Read User
                    new PermissionId(Guid.Parse("22222222-2222-2222-2222-222222222222")), // Read Subject
                    new PermissionId(Guid.Parse("33333333-3333-3333-3333-333333333332")), // Read Role
                    new PermissionId(Guid.Parse("44444444-4444-4444-4444-444444444442")), // Read Permission
                    new PermissionId(Guid.Parse("55555555-5555-5555-5555-555555555551")), // View Sessions
                    
                    // Audit and Compliance
                    new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd01")), // View Audit Logs
                    new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd02")), // Export Audit Data
                    new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd03")), // Compliance Reporting
                    new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd04")), // Data Retention Management
                    new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd05")), // Privacy Controls
                    
                    // Analytics and Reporting
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")), // View Analytics Dashboard
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02")), // Generate Custom Reports
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff03")), // Export Data
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04")), // View Usage Statistics
                    new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff05")), // Performance Monitoring
                    
                    // Security (view only)
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")), // View Security Events
                    new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998"))  // Audit Security
                },
                status: RoleStatus.Active
            )
        };

        foreach (var role in roles)
        {
            await _roleRepository.AddAsync(role);
        }
        
        _logger.LogInformation("Added {Count} roles", roles.Count);
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
            
            // Subject Management
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
            
            // Citizen Services
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777771")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777772")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777773")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777774")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777775")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777776")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777777")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777778")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-777777777779")),
            new PermissionId(Guid.Parse("77777777-7777-7777-7777-77777777777a")),
            
            // Staff Services
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888881")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888882")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888883")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888884")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888885")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888886")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888887")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888888")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-888888888889")),
            new PermissionId(Guid.Parse("88888888-8888-8888-8888-88888888888a")),
            
            // Security
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999991")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999992")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999993")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999994")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999995")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999996")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999997")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999998")),
            new PermissionId(Guid.Parse("99999999-9999-9999-9999-999999999999")),
            
            // Government Services
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa06")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa07")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa08")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa09")),
            new PermissionId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa10")),
            
            // Financial
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01")),
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02")),
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03")),
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04")),
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05")),
            new PermissionId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06")),
            
            // Communication
            new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc01")),
            new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc02")),
            new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc03")),
            new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc04")),
            new PermissionId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc05")),
            
            // Audit and Compliance
            new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd01")),
            new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd02")),
            new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd03")),
            new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd04")),
            new PermissionId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd05")),
            
            // Integration
            new PermissionId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee01")),
            new PermissionId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee02")),
            new PermissionId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee03")),
            new PermissionId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeee04")),
            
            // Analytics and Reporting
            new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff01")),
            new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff02")),
            new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff03")),
            new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff04")),
            new PermissionId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffff05")),
            
            // Emergency and Disaster Recovery
            new PermissionId(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new PermissionId(Guid.Parse("00000000-0000-0000-0000-000000000002")),
            new PermissionId(Guid.Parse("00000000-0000-0000-0000-000000000003")),
            new PermissionId(Guid.Parse("00000000-0000-0000-0000-000000000004")),
            new PermissionId(Guid.Parse("00000000-0000-0000-0000-000000000005"))
        };
    }

    private async Task AddSubjectsAsync()
    {
        _logger.LogInformation("Adding subjects...");

        var subjects = new List<Subject>
        {
            // SuperAdmin Subject
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                "System Administrator",
                "admin@mamey.io",
                tags: new[] { "system", "admin", "super" },
                roles: new[] { new RoleId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")) }
            ),

            // System Administrator Subject
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffe"),
                "System Administrator",
                "sysadmin@mamey.io",
                tags: new[] { "system", "admin", "technical" },
                roles: new[] { new RoleId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")) }
            ),

            // Government Administrator Subject
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffd"),
                "Government Administrator",
                "govadmin@mamey.io",
                tags: new[] { "admin", "government", "management" },
                roles: new[] { new RoleId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")) }
            ),

            // Department Manager - Immigration Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffc"),
                "Immigration Services Manager",
                "immigration.manager@mamey.io",
                tags: new[] { "manager", "immigration", "passport", "visa" },
                roles: new[] { new RoleId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")) }
            ),

            // Department Manager - Citizen Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffb"),
                "Citizen Services Manager",
                "citizen.manager@mamey.io",
                tags: new[] { "manager", "citizen-services", "birth-certificate", "id-card" },
                roles: new[] { new RoleId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")) }
            ),

            // Senior Staff - Passport Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffa"),
                "Passport Services Senior Staff",
                "passport.senior@mamey.io",
                tags: new[] { "staff", "senior", "passport", "immigration" },
                roles: new[] { new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")) }
            ),

            // Staff - ID Card Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff9"),
                "ID Card Services Staff",
                "idcard.staff@mamey.io",
                tags: new[] { "staff", "id-card", "citizen-services" },
                roles: new[] { new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")) }
            ),

            // Staff - Birth Certificate Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff8"),
                "Birth Certificate Services Staff",
                "birth.staff@mamey.io",
                tags: new[] { "staff", "birth-certificate", "citizen-services" },
                roles: new[] { new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")) }
            ),

            // Staff - Tax Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff7"),
                "Tax Services Staff",
                "tax.staff@mamey.io",
                tags: new[] { "staff", "tax", "financial" },
                roles: new[] { new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")) }
            ),

            // Staff - Social Security Services
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff6"),
                "Social Security Services Staff",
                "social.staff@mamey.io",
                tags: new[] { "staff", "social-security", "benefits" },
                roles: new[] { new RoleId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")) }
            ),

            // Auditor Subject
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff5"),
                "System Auditor",
                "auditor@mamey.io",
                tags: new[] { "auditor", "compliance", "read-only" },
                roles: new[] { new RoleId(Guid.Parse("11111111-1111-1111-1111-111111111111")) }
            ),

            // Citizen - John Smith
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff4"),
                "John Smith",
                "john.smith@example.com",
                tags: new[] { "citizen", "verified", "frequent-user" },
                roles: new[] { new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")) }
            ),

            // Citizen - Jane Doe
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff3"),
                "Jane Doe",
                "jane.doe@example.com",
                tags: new[] { "citizen", "new", "business-owner" },
                roles: new[] { new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")) }
            ),

            // Citizen - Robert Johnson
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2"),
                "Robert Johnson",
                "robert.johnson@example.com",
                tags: new[] { "citizen", "verified", "senior" },
                roles: new[] { new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")) }
            ),

            // Citizen - Maria Garcia
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1"),
                "Maria Garcia",
                "maria.garcia@example.com",
                tags: new[] { "citizen", "verified", "immigrant" },
                roles: new[] { new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")) }
            ),

            // Citizen - David Wilson
            Subject.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff0"),
                "David Wilson",
                "david.wilson@example.com",
                tags: new[] { "citizen", "new", "student" },
                roles: new[] { new RoleId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")) }
            )
        };

        foreach (var subject in subjects)
        {
            await _subjectRepository.AddAsync(subject);
        }
        
        _logger.LogInformation("Added {Count} subjects", subjects.Count);
    }

    private async Task AddUsersAsync()
    {
        _logger.LogInformation("Adding users...");

        var users = new List<User>
        {
            // SuperAdmin User
            User.Create(
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                "superadmin",
                "admin@mamey.io",
                _hasher.Hash("SuperAdmin123!")
            ),

            // System Administrator
            User.Create(
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffe"),
                "sysadmin",
                "sysadmin@mamey.io",
                _hasher.Hash("SysAdmin123!")
            ),

            // Government Administrator
            User.Create(
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffd"),
                "govadmin",
                "govadmin@mamey.io",
                _hasher.Hash("GovAdmin123!")
            ),

            // Department Manager - Immigration Services
            User.Create(
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffc"),
                "immigration.manager",
                "immigration.manager@mamey.io",
                _hasher.Hash("ImmManager123!")
            ),

            // Department Manager - Citizen Services
            User.Create(
                Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffb"),
                "citizen.manager",
                "citizen.manager@mamey.io",
                _hasher.Hash("CitManager123!")
            ),

            // Senior Staff - Passport Services
            User.Create(
                Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffa"),
                "passport.senior",
                "passport.senior@mamey.io",
                _hasher.Hash("PassSenior123!")
            ),

            // Staff - ID Card Services
            User.Create(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff9"),
                "idcard.staff",
                "idcard.staff@mamey.io",
                _hasher.Hash("IdStaff123!")
            ),

            // Staff - Birth Certificate Services
            User.Create(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff8"),
                "birth.staff",
                "birth.staff@mamey.io",
                _hasher.Hash("BirthStaff123!")
            ),

            // Staff - Tax Services
            User.Create(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff7"),
                "tax.staff",
                "tax.staff@mamey.io",
                _hasher.Hash("TaxStaff123!")
            ),

            // Staff - Social Security Services
            User.Create(
                Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff6"),
                "social.staff",
                "social.staff@mamey.io",
                _hasher.Hash("SocialStaff123!")
            ),

            // Auditor
            User.Create(
                Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff5"),
                "auditor",
                "auditor@mamey.io",
                _hasher.Hash("Auditor123!")
            ),

            // Citizen - John Smith
            User.Create(
                Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff4"),
                "john.smith",
                "john.smith@example.com",
                _hasher.Hash("JohnSmith123!")
            ),

            // Citizen - Jane Doe
            User.Create(
                Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff3"),
                "jane.doe",
                "jane.doe@example.com",
                _hasher.Hash("JaneDoe123!")
            ),

            // Citizen - Robert Johnson
            User.Create(
                Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2"),
                "robert.johnson",
                "robert.johnson@example.com",
                _hasher.Hash("Robert123!")
            ),

            // Citizen - Maria Garcia
            User.Create(
                Guid.Parse("99999999-9999-9999-9999-999999999999"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1"),
                "maria.garcia",
                "maria.garcia@example.com",
                _hasher.Hash("Maria123!")
            ),

            // Citizen - David Wilson
            User.Create(
                Guid.Parse("00000000-0000-0000-0000-000000000000"),
                Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff0"),
                "david.wilson",
                "david.wilson@example.com",
                _hasher.Hash("David123!")
            )
        };

        foreach (var user in users)
        {
            await _userRepository.AddAsync(user);
        }
        
        _logger.LogInformation("Added {Count} users", users.Count);
    }

    private async Task AddCredentialsAsync()
    {
        _logger.LogInformation("Adding credentials...");

        var credentials = new List<Credential>
        {
            // SuperAdmin Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")),
                CredentialType.Password,
                _hasher.Hash("SuperAdmin123!"),
                DateTime.UtcNow.AddDays(-30),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(365)
            ),

            // System Administrator Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")),
                CredentialType.Password,
                _hasher.Hash("SysAdmin123!"),
                DateTime.UtcNow.AddDays(-25),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(180)
            ),

            // Government Administrator Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")),
                CredentialType.Password,
                _hasher.Hash("GovAdmin123!"),
                DateTime.UtcNow.AddDays(-20),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(180)
            ),

            // Department Manager - Immigration Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")),
                CredentialType.Password,
                _hasher.Hash("ImmManager123!"),
                DateTime.UtcNow.AddDays(-15),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(120)
            ),

            // Department Manager - Citizen Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee")),
                CredentialType.Password,
                _hasher.Hash("CitManager123!"),
                DateTime.UtcNow.AddDays(-12),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(120)
            ),

            // Senior Staff - Passport Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff")),
                CredentialType.Password,
                _hasher.Hash("PassSenior123!"),
                DateTime.UtcNow.AddDays(-10),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(90)
            ),

            // Staff - ID Card Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("11111111-1111-1111-1111-111111111111")),
                CredentialType.Password,
                _hasher.Hash("IdStaff123!"),
                DateTime.UtcNow.AddDays(-8),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(90)
            ),

            // Staff - Birth Certificate Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("22222222-2222-2222-2222-222222222222")),
                CredentialType.Password,
                _hasher.Hash("BirthStaff123!"),
                DateTime.UtcNow.AddDays(-6),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(90)
            ),

            // Staff - Tax Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("33333333-3333-3333-3333-333333333333")),
                CredentialType.Password,
                _hasher.Hash("TaxStaff123!"),
                DateTime.UtcNow.AddDays(-5),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(90)
            ),

            // Staff - Social Security Services Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("44444444-4444-4444-4444-444444444444")),
                CredentialType.Password,
                _hasher.Hash("SocialStaff123!"),
                DateTime.UtcNow.AddDays(-4),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(90)
            ),

            // Auditor Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("55555555-5555-5555-5555-555555555555")),
                CredentialType.Password,
                _hasher.Hash("Auditor123!"),
                DateTime.UtcNow.AddDays(-3),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            ),

            // Citizen - John Smith Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("66666666-6666-6666-6666-666666666666")),
                CredentialType.Password,
                _hasher.Hash("JohnSmith123!"),
                DateTime.UtcNow.AddDays(-2),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            ),

            // Citizen - Jane Doe Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("77777777-7777-7777-7777-777777777777")),
                CredentialType.Password,
                _hasher.Hash("JaneDoe123!"),
                DateTime.UtcNow.AddDays(-1),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            ),

            // Citizen - Robert Johnson Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("88888888-8888-8888-8888-888888888888")),
                CredentialType.Password,
                _hasher.Hash("Robert123!"),
                DateTime.UtcNow.AddHours(-12),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            ),

            // Citizen - Maria Garcia Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("99999999-9999-9999-9999-999999999999")),
                CredentialType.Password,
                _hasher.Hash("Maria123!"),
                DateTime.UtcNow.AddHours(-6),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            ),

            // Citizen - David Wilson Credentials
            new Credential(
                new CredentialId(Guid.NewGuid()),
                new UserId(Guid.Parse("00000000-0000-0000-0000-000000000000")),
                CredentialType.Password,
                _hasher.Hash("David123!"),
                DateTime.UtcNow.AddHours(-3),
                modifiedAt: null,
                expiresAt: DateTime.UtcNow.AddDays(60)
            )
        };

        foreach (var credential in credentials)
        {
            await _credentialRepository.AddAsync(credential);
        }
        
        _logger.LogInformation("Added {Count} credentials", credentials.Count);
    }

    // private static string HashPassword(string password)
    // {
    //     using var sha256 = SHA256.Create();
    //     var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    //     return Convert.ToBase64String(hashedBytes);
    // }
}