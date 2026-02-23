using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AIVision.Domain.Exceptions;

internal class MissingVisionAnalysisNameException : DomainException
{
    public MissingVisionAnalysisNameException()
        : base("VisionAnalysis name is missing.")
    {
    }

    public override string Code => "missing_visionanalysis_name";
}
