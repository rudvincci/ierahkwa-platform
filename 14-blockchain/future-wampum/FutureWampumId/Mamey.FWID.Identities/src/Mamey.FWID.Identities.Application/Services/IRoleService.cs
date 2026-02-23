using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Types;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling role operations.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="name">The role name.</param>
    /// <param name="description">The role description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created role identifier.</returns>
    Task<RoleId> CreateRoleAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="name">The new role name.</param>
    /// <param name="description">The new role description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdateRoleAsync(
        RoleId roleId,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteRoleAsync(
        RoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AssignRoleToIdentityAsync(
        IdentityId identityId,
        RoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RemoveRoleFromIdentityAsync(
        IdentityId identityId,
        RoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a permission to a role.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddPermissionToRoleAsync(
        RoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a permission from a role.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RemovePermissionFromRoleAsync(
        RoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of role identifiers.</returns>
    Task<List<RoleId>> GetIdentityRolesAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default);
}

