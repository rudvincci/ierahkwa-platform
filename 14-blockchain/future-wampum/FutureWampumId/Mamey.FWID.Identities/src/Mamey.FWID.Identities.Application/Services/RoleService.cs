using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling role operations.
/// </summary>
internal sealed class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IIdentityRoleRepository _identityRoleRepository;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        IRoleRepository roleRepository,
        IIdentityRoleRepository identityRoleRepository,
        ILogger<RoleService> logger)
    {
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _identityRoleRepository = identityRoleRepository ?? throw new ArgumentNullException(nameof(identityRoleRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RoleId> CreateRoleAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        // Check if role already exists
        var existing = await _roleRepository.GetByNameAsync(name, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException($"Role with name '{name}' already exists");

        var roleId = new RoleId();
        var role = new Role(roleId, name, description);
        await _roleRepository.AddAsync(role, cancellationToken);

        _logger.LogInformation("Role created: {RoleId}, name: {Name}", roleId, name);

        return roleId;
    }

    public async Task UpdateRoleAsync(
        RoleId roleId,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be null or empty", nameof(name));

        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Role not found");

        role.Update(name, description);
        await _roleRepository.UpdateAsync(role, cancellationToken);

        _logger.LogInformation("Role updated: {RoleId}", roleId);
    }

    public async Task DeleteRoleAsync(
        RoleId roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null)
            return; // Already deleted

        // Check if role is assigned to any identities
        var assignments = await _identityRoleRepository.GetByRoleIdAsync(roleId, cancellationToken);
        if (assignments.Any())
            throw new InvalidOperationException("Cannot delete role that is assigned to identities");

        await _roleRepository.DeleteAsync(roleId, cancellationToken);

        _logger.LogInformation("Role deleted: {RoleId}", roleId);
    }

    public async Task AssignRoleToIdentityAsync(
        IdentityId identityId,
        RoleId roleId,
        CancellationToken cancellationToken = default)
    {
        // Check if already assigned
        var existing = await _identityRoleRepository.GetByIdentityAndRoleAsync(
            identityId, roleId, cancellationToken);

        if (existing != null)
            return; // Already assigned

        var identityRole = new IdentityRole(identityId, roleId);
        await _identityRoleRepository.AddAsync(identityRole, cancellationToken);

        _logger.LogInformation("Role {RoleId} assigned to identity {IdentityId}", roleId, identityId);
    }

    public async Task RemoveRoleFromIdentityAsync(
        IdentityId identityId,
        RoleId roleId,
        CancellationToken cancellationToken = default)
    {
        var identityRole = await _identityRoleRepository.GetByIdentityAndRoleAsync(
            identityId, roleId, cancellationToken);

        if (identityRole == null)
            return; // Not assigned

        await _identityRoleRepository.DeleteAsync(identityRole.Id, cancellationToken);

        _logger.LogInformation("Role {RoleId} removed from identity {IdentityId}", roleId, identityId);
    }

    public async Task AddPermissionToRoleAsync(
        RoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Role not found");

        role.AddPermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);

        _logger.LogInformation("Permission {PermissionId} added to role {RoleId}", permissionId, roleId);
    }

    public async Task RemovePermissionFromRoleAsync(
        RoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetAsync(roleId, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Role not found");

        role.RemovePermission(permissionId);
        await _roleRepository.UpdateAsync(role, cancellationToken);

        _logger.LogInformation("Permission {PermissionId} removed from role {RoleId}", permissionId, roleId);
    }

    public async Task<List<RoleId>> GetIdentityRolesAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default)
    {
        var identityRoles = await _identityRoleRepository.GetByIdentityIdAsync(identityId, cancellationToken);
        return identityRoles.Select(ir => ir.RoleId).ToList();
    }
}

