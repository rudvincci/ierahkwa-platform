using Mamey.Government.Identity.Application.Commands;
using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Identity.Application.Services;

internal interface IRoleService
{
    #region Role CRUD Operations
    Task<RoleDto?> GetRoleAsync(RoleId id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateRoleAsync(CreateRole command, CancellationToken cancellationToken = default);
    Task<RoleDto> UpdateRoleAsync(UpdateRole command, CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(RoleId id, CancellationToken cancellationToken = default);
    #endregion

    #region Role Assignment Management
    Task AssignRoleToSubjectAsync(AssignRoleToSubject command, CancellationToken cancellationToken = default);
    Task RemoveRoleFromSubjectAsync(RemoveRoleFromSubject command, CancellationToken cancellationToken = default);
    #endregion

    #region Role Search and Filtering
    Task<IEnumerable<RoleDto>> GetRolesByStatusAsync(RoleStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleDto>> GetRolesByPermissionAsync(PermissionId permissionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm, CancellationToken cancellationToken = default);
    #endregion

    #region Role Statistics
    Task<RoleStatisticsDto> GetRoleStatisticsAsync(CancellationToken cancellationToken = default);
    #endregion

    #region Role Permission Management
    Task AddPermissionToRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(RoleId roleId, PermissionId permissionId, CancellationToken cancellationToken = default);
    #endregion
}
