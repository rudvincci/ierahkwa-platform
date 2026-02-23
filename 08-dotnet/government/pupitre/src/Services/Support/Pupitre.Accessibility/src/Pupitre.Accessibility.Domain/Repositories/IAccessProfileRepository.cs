using System;
using Pupitre.Accessibility.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Accessibility.Domain.Repositories;

internal interface IAccessProfileRepository
{
    Task AddAsync(AccessProfile accessprofile, CancellationToken cancellationToken = default);
    Task UpdateAsync(AccessProfile accessprofile, CancellationToken cancellationToken = default);
    Task DeleteAsync(AccessProfileId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccessProfile>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<AccessProfile> GetAsync(AccessProfileId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AccessProfileId id, CancellationToken cancellationToken = default);
}
