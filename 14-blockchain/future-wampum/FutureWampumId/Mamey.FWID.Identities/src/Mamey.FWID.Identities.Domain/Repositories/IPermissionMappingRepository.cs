using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Repositories;

/// <summary>
/// Repository interface for permission mappings.
/// </summary>
internal interface IPermissionMappingRepository
{
    Task AddAsync(PermissionMapping mapping, CancellationToken cancellationToken = default);
    Task UpdateAsync(PermissionMapping mapping, CancellationToken cancellationToken = default);
    Task<PermissionMapping?> GetByServiceNameAsync(string serviceName, CancellationToken cancellationToken = default);
    Task<PermissionMapping?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermissionMapping>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

