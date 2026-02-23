using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Compliance.Domain.Repositories;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Compliance.Infrastructure.Mongo.Repositories;

internal class ComplianceRecordMongoRepository : IComplianceRecordRepository
{
    private readonly IMongoRepository<ComplianceRecordDocument, Guid> _repository;

    public ComplianceRecordMongoRepository(IMongoRepository<ComplianceRecordDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(ComplianceRecord compliancerecord, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new ComplianceRecordDocument(compliancerecord));

    public async Task UpdateAsync(ComplianceRecord compliancerecord, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new ComplianceRecordDocument(compliancerecord));
    public async Task DeleteAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<ComplianceRecord>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<ComplianceRecord> GetAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        var compliancerecord = await _repository.GetAsync(id.Value);
        return compliancerecord?.AsEntity();
    }
    public async Task<bool> ExistsAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



