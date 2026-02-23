using System;
using Pupitre.AISafety.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AISafety.Domain.Repositories;

internal interface ISafetyCheckRepository
{
    Task AddAsync(SafetyCheck safetycheck, CancellationToken cancellationToken = default);
    Task UpdateAsync(SafetyCheck safetycheck, CancellationToken cancellationToken = default);
    Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default);
}
