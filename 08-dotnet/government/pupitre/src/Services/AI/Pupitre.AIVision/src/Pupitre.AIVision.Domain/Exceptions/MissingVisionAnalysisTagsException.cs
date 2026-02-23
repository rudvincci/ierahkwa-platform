using Mamey.Exceptions;

namespace Pupitre.AIVision.Domain.Exceptions;

internal class MissingVisionAnalysisTagsException : DomainException
{
    public MissingVisionAnalysisTagsException()
        : base("VisionAnalysis tags are missing.")
    {
    }

    public override string Code => "missing_visionanalysis_tags";
}