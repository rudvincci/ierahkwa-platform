using System;
using Pupitre.Operations.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Operations.Domain.Repositories;

internal interface IOperationMetricRepository
{
    Task AddAsync(OperationMetric operationmetric, CancellationToken cancellationToken = default);
    Task UpdateAsync(OperationMetric operationmetric, CancellationToken cancellationToken = default);
    Task DeleteAsync(OperationMetricId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OperationMetric>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<OperationMetric> GetAsync(OperationMetricId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(OperationMetricId id, CancellationToken cancellationToken = default);
}
