using System;
using Pupitre.Assessments.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Assessments.Domain.Repositories;

internal interface IAssessmentRepository
{
    Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken = default);
    Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default);
}
