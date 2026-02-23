using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIVision.Domain.Entities;

namespace Pupitre.AIVision.Application.Exceptions;

internal class VisionAnalysisAlreadyExistsException : MameyException
{
    public VisionAnalysisAlreadyExistsException(VisionAnalysisId visionanalysisId)
        : base($"VisionAnalysis with ID: '{visionanalysisId.Value}' already exists.")
        => VisionAnalysisId = visionanalysisId;

    public VisionAnalysisId VisionAnalysisId { get; }
}
