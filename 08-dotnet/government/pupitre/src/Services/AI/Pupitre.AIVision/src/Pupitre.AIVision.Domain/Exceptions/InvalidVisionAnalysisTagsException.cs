using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIVision.Domain.Exceptions;

internal class InvalidVisionAnalysisTagsException : DomainException
{
    public override string Code { get; } = "invalid_visionanalysis_tags";

    public InvalidVisionAnalysisTagsException() : base("VisionAnalysis tags are invalid.")
    {
    }
}
