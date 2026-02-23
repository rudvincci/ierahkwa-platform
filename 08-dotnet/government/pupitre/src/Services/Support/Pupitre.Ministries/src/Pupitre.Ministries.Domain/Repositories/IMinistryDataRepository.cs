using System;
using Pupitre.Ministries.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Ministries.Domain.Repositories;

internal interface IMinistryDataRepository
{
    Task AddAsync(MinistryData ministrydata, CancellationToken cancellationToken = default);
    Task UpdateAsync(MinistryData ministrydata, CancellationToken cancellationToken = default);
    Task DeleteAsync(MinistryDataId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MinistryData>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<MinistryData> GetAsync(MinistryDataId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(MinistryDataId id, CancellationToken cancellationToken = default);
}
