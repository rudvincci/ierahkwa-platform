using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIVision.Domain.Entities;

namespace Pupitre.AIVision.Application.Exceptions;

internal class VisionAnalysisNotFoundException : MameyException
{
    public VisionAnalysisNotFoundException(VisionAnalysisId visionanalysisId)
        : base($"VisionAnalysis with ID: '{visionanalysisId.Value}' was not found.")
        => VisionAnalysisId = visionanalysisId;

    public VisionAnalysisId VisionAnalysisId { get; }
}

