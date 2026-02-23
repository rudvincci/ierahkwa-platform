using Mamey.Exceptions;

namespace Pupitre.Assessments.Domain.Exceptions;

internal class MissingAssessmentTagsException : DomainException
{
    public MissingAssessmentTagsException()
        : base("Assessment tags are missing.")
    {
    }

    public override string Code => "missing_assessment_tags";
}