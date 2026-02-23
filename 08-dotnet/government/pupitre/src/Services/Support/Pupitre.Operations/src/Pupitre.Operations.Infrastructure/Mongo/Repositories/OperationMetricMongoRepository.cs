using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Operations.Domain.Repositories;
using Pupitre.Operations.Domain.Entities;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Operations.Infrastructure.Mongo.Repositories;

internal class OperationMetricMongoRepository : IOperationMetricRepository
{
    private readonly IMongoRepository<OperationMetricDocument, Guid> _repository;

    public OperationMetricMongoRepository(IMongoRepository<OperationMetricDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(OperationMetric operationmetric, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new OperationMetricDocument(operationmetric));

    public async Task UpdateAsync(OperationMetric operationmetric, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new OperationMetricDocument(operationmetric));
    public async Task DeleteAsync(OperationMetricId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<OperationMetric>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<OperationMetric> GetAsync(OperationMetricId id, CancellationToken cancellationToken = default)
    {
        var operationmetric = await _repository.GetAsync(id.Value);
        return operationmetric?.AsEntity();
    }
    public async Task<bool> ExistsAsync(OperationMetricId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



