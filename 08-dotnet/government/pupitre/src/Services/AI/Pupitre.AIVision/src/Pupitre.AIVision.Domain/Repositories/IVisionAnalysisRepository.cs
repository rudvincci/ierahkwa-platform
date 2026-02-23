using System;
using Pupitre.AIVision.Domain.Entities;
using Mamey.Types;

namespace Pupitre.AIVision.Domain.Repositories;

internal interface IVisionAnalysisRepository
{
    Task AddAsync(VisionAnalysis visionanalysis, CancellationToken cancellationToken = default);
    Task UpdateAsync(VisionAnalysis visionanalysis, CancellationToken cancellationToken = default);
    Task DeleteAsync(VisionAnalysisId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VisionAnalysis>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<VisionAnalysis> GetAsync(VisionAnalysisId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(VisionAnalysisId id, CancellationToken cancellationToken = default);
}
