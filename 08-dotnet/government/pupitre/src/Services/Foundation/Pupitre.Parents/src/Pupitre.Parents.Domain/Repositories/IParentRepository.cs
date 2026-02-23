using System;
using Pupitre.Parents.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Parents.Domain.Repositories;

internal interface IParentRepository
{
    Task AddAsync(Parent parent, CancellationToken cancellationToken = default);
    Task UpdateAsync(Parent parent, CancellationToken cancellationToken = default);
    Task DeleteAsync(ParentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Parent>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Parent> GetAsync(ParentId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(ParentId id, CancellationToken cancellationToken = default);
}
