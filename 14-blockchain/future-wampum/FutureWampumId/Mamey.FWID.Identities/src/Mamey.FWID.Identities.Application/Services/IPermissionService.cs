using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling permission operations.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="name">The permission name.</param>
    /// <param name="description">The permission description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created permission identifier.</returns>
    Task<PermissionId> CreatePermissionAsync(
        string name,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a permission.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="name">The new permission name.</param>
    /// <param name="description">The new permission description.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task UpdatePermissionAsync(
        PermissionId permissionId,
        string name,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeletePermissionAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a permission to an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AssignPermissionToIdentityAsync(
        IdentityId identityId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a permission from an identity.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task RemovePermissionFromIdentityAsync(
        IdentityId identityId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an identity has a specific permission.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="permissionName">The permission name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the identity has the permission.</returns>
    Task<bool> HasPermissionAsync(
        IdentityId identityId,
        string permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for an identity (direct and via roles).
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The list of permission identifiers.</returns>
    Task<List<PermissionId>> GetIdentityPermissionsAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default);
}

