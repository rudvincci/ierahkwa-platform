using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling permission operations.
/// </summary>
internal sealed class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IIdentityPermissionRepository _identityPermissionRepository;
    private readonly IIdentityRoleRepository _identityRoleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IPermissionRepository permissionRepository,
        IIdentityPermissionRepository identityPermissionRepository,
        IIdentityRoleRepository identityRoleRepository,
        IRoleRepository roleRepository,
        ILogger<PermissionService> logger)
    {
        _permissionRepository = permissionRepository ?? throw new ArgumentNullException(nameof(permissionRepository));
        _identityPermissionRepository = identityPermissionRepository ?? throw new ArgumentNullException(nameof(identityPermissionRepository));
        _identityRoleRepository = identityRoleRepository ?? throw new ArgumentNullException(nameof(identityRoleRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PermissionId> CreatePermissionAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be null or empty", nameof(name));

        // Check if permission already exists
        var existing = await _permissionRepository.GetByNameAsync(name, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Permission with name '{name}' already exists");

        var permissionId = new PermissionId();
        var permission = new Permission(permissionId, name, description);
        await _permissionRepository.AddAsync(permission, cancellationToken);

        _logger.LogInformation("Permission created: {PermissionId}, name: {Name}", permissionId, name);

        return permissionId;
    }

    public async Task UpdatePermissionAsync(
        PermissionId permissionId,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be null or empty", nameof(name));

        var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
        if (permission == null)
            throw new InvalidOperationException("Permission not found");

        permission.Update(name, description);
        await _permissionRepository.UpdateAsync(permission, cancellationToken);

        _logger.LogInformation("Permission updated: {PermissionId}", permissionId);
    }

    public async Task DeletePermissionAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default)
    {
        var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
        if (permission == null)
            return; // Already deleted

        // Check if permission is assigned to any identities
        var assignments = await _identityPermissionRepository.GetByPermissionIdAsync(permissionId, cancellationToken);
        if (assignments.Any())
            throw new InvalidOperationException("Cannot delete permission that is assigned to identities");

        await _permissionRepository.DeleteAsync(permissionId, cancellationToken);

        _logger.LogInformation("Permission deleted: {PermissionId}", permissionId);
    }

    public async Task AssignPermissionToIdentityAsync(
        IdentityId identityId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default)
    {
        // Check if already assigned
        var existing = await _identityPermissionRepository.GetByIdentityAndPermissionAsync(
            identityId, permissionId, cancellationToken);

        if (existing != null)
            return; // Already assigned

        var identityPermission = new IdentityPermission(identityId, permissionId);
        await _identityPermissionRepository.AddAsync(identityPermission, cancellationToken);

        _logger.LogInformation("Permission {PermissionId} assigned to identity {IdentityId}", permissionId, identityId);
    }

    public async Task RemovePermissionFromIdentityAsync(
        IdentityId identityId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default)
    {
        var identityPermission = await _identityPermissionRepository.GetByIdentityAndPermissionAsync(
            identityId, permissionId, cancellationToken);

        if (identityPermission == null)
            return; // Not assigned

        await _identityPermissionRepository.DeleteAsync(identityPermission.Id, cancellationToken);

        _logger.LogInformation("Permission {PermissionId} removed from identity {IdentityId}", permissionId, identityId);
    }

    public async Task<bool> HasPermissionAsync(
        IdentityId identityId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetIdentityPermissionsAsync(identityId, cancellationToken);
        
        foreach (var permissionId in permissions)
        {
            var permission = await _permissionRepository.GetAsync(permissionId, cancellationToken);
            if (permission != null && permission.Name == permissionName && permission.Status == Domain.ValueObjects.PermissionStatus.Active)
                return true;
        }

        return false;
    }

    public async Task<List<PermissionId>> GetIdentityPermissionsAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default)
    {
        var permissions = new HashSet<PermissionId>();

        // Get direct permissions
        var directPermissions = await _identityPermissionRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        foreach (var assignment in directPermissions)
        {
            permissions.Add(assignment.PermissionId);
        }

        // Get permissions via roles
        var identityRoles = await _identityRoleRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        foreach (var identityRole in identityRoles)
        {
            var role = await _roleRepository.GetAsync(identityRole.RoleId, cancellationToken);
            if (role != null && role.Status == Domain.ValueObjects.RoleStatus.Active)
            {
                foreach (var permissionId in role.Permissions)
                {
                    permissions.Add(permissionId);
                }
            }
        }

        return permissions.ToList();
    }
}

