using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IPermissionService
{
    #region Permission CRUD Operations
    Task<PermissionDto?> GetPermissionAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task<PermissionDto> CreatePermissionAsync(CreatePermission command, CancellationToken cancellationToken = default);
    Task<PermissionDto> UpdatePermissionAsync(UpdatePermission command, CancellationToken cancellationToken = default);
    Task DeletePermissionAsync(PermissionId id, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Search and Filtering
    Task<IEnumerable<PermissionDto>> GetPermissionsByStatusAsync(PermissionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<PermissionDto>> GetPermissionsByResourceAsync(string resource, CancellationToken cancellationToken = default);
    Task<IEnumerable<PermissionDto>> GetPermissionsByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<PermissionDto>> GetPermissionsByRoleAsync(RoleId roleId, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Management
    Task ActivatePermissionAsync(PermissionId id, CancellationToken cancellationToken = default);
    Task DeactivatePermissionAsync(PermissionId id, CancellationToken cancellationToken = default);
    #endregion

    #region Permission Statistics
    Task<PermissionStatisticsDto> GetPermissionStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion
}
