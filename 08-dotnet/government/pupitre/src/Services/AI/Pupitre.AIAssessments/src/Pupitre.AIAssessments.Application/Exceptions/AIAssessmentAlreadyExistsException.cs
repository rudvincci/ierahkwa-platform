using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIAssessments.Domain.Entities;

namespace Pupitre.AIAssessments.Application.Exceptions;

internal class AIAssessmentAlreadyExistsException : MameyException
{
    public AIAssessmentAlreadyExistsException(AIAssessmentId aiassessmentId)
        : base($"AIAssessment with ID: '{aiassessmentId.Value}' already exists.")
        => AIAssessmentId = aiassessmentId;

    public AIAssessmentId AIAssessmentId { get; }
}
