using System;
using Pupitre.AIAssessments.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIAssessments.Domain.Repositories;

internal interface IAIAssessmentRepository
{
    Task AddAsync(AIAssessment aiassessment, CancellationToken cancellationToken = default);
    Task UpdateAsync(AIAssessment aiassessment, CancellationToken cancellationToken = default);
    Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default);
}
