using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Assessments.Domain.Exceptions;

internal class InvalidAssessmentTagsException : DomainException
{
    public override string Code { get; } = "invalid_assessment_tags";

    public InvalidAssessmentTagsException() : base("Assessment tags are invalid.")
    {
    }
}
