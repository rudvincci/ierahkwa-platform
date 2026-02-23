using System;
using Mamey.ServiceName.Domain.Entities;
using Mamey.Types;

namespace Mamey.ServiceName.Domain.Repositories;

internal interface IEntityNameRepository
{
    Task AddAsync(EntityName entityname, CancellationToken cancellationToken = default);
    Task UpdateAsync(EntityName entityname, CancellationToken cancellationToken = default);
    Task DeleteAsync(EntityNameId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EntityName>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<EntityName> GetAsync(EntityNameId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(EntityNameId id, CancellationToken cancellationToken = default);
}
