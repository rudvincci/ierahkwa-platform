using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Assessments.Domain.Entities;

namespace Pupitre.Assessments.Application.Exceptions;

internal class AssessmentNotFoundException : MameyException
{
    public AssessmentNotFoundException(AssessmentId assessmentId)
        : base($"Assessment with ID: '{assessmentId.Value}' was not found.")
        => AssessmentId = assessmentId;

    public AssessmentId AssessmentId { get; }
}

