using System;
using Pupitre.Compliance.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Compliance.Domain.Repositories;

internal interface IComplianceRecordRepository
{
    Task AddAsync(ComplianceRecord compliancerecord, CancellationToken cancellationToken = default);
    Task UpdateAsync(ComplianceRecord compliancerecord, CancellationToken cancellationToken = default);
    Task DeleteAsync(ComplianceRecordId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ComplianceRecord>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<ComplianceRecord> GetAsync(ComplianceRecordId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(ComplianceRecordId id, CancellationToken cancellationToken = default);
}
