using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Assessments.Domain.Entities;

namespace Pupitre.Assessments.Application.Exceptions;

internal class AssessmentAlreadyExistsException : MameyException
{
    public AssessmentAlreadyExistsException(AssessmentId assessmentId)
        : base($"Assessment with ID: '{assessmentId.Value}' already exists.")
        => AssessmentId = assessmentId;

    public AssessmentId AssessmentId { get; }
}
