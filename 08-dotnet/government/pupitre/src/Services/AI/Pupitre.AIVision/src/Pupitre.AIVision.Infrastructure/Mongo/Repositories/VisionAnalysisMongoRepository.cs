using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIVision.Domain.Repositories;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIVision.Infrastructure.Mongo.Repositories;

internal class VisionAnalysisMongoRepository : IVisionAnalysisRepository
{
    private readonly IMongoRepository<VisionAnalysisDocument, Guid> _repository;

    public VisionAnalysisMongoRepository(IMongoRepository<VisionAnalysisDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(VisionAnalysis visionanalysis, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new VisionAnalysisDocument(visionanalysis));

    public async Task UpdateAsync(VisionAnalysis visionanalysis, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new VisionAnalysisDocument(visionanalysis));
    public async Task DeleteAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<VisionAnalysis>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<VisionAnalysis> GetAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        var visionanalysis = await _repository.GetAsync(id.Value);
        return visionanalysis?.AsEntity();
    }
    public async Task<bool> ExistsAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



