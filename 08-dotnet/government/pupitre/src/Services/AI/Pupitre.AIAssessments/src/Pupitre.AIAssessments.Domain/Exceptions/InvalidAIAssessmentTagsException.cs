using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.AIAssessments.Domain.Exceptions;

internal class InvalidAIAssessmentTagsException : DomainException
{
    public override string Code { get; } = "invalid_aiassessment_tags";

    public InvalidAIAssessmentTagsException() : base("AIAssessment tags are invalid.")
    {
    }
}
