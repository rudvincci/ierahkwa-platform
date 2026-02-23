using System;
using Pupitre.AITutors.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AITutors.Domain.Repositories;

internal interface ITutorRepository
{
    Task AddAsync(Tutor tutor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tutor tutor, CancellationToken cancellationToken = default);
    Task DeleteAsync(TutorId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tutor>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Tutor> GetAsync(TutorId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TutorId id, CancellationToken cancellationToken = default);
}
