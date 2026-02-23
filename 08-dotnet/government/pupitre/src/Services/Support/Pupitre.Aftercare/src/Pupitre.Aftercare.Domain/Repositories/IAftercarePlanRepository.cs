using System;
using Pupitre.Aftercare.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Aftercare.Domain.Repositories;

internal interface IAftercarePlanRepository
{
    Task AddAsync(AftercarePlan aftercareplan, CancellationToken cancellationToken = default);
    Task UpdateAsync(AftercarePlan aftercareplan, CancellationToken cancellationToken = default);
    Task DeleteAsync(AftercarePlanId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AftercarePlan>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<AftercarePlan> GetAsync(AftercarePlanId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AftercarePlanId id, CancellationToken cancellationToken = default);
}
