using Mamey.Exceptions;

namespace Pupitre.AIAssessments.Domain.Exceptions;

internal class MissingAIAssessmentTagsException : DomainException
{
    public MissingAIAssessmentTagsException()
        : base("AIAssessment tags are missing.")
    {
    }

    public override string Code => "missing_aiassessment_tags";
}