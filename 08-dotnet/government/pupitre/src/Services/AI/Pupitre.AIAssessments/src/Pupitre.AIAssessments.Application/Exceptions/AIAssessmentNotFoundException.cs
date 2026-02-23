using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.AIAssessments.Domain.Entities;

namespace Pupitre.AIAssessments.Application.Exceptions;

internal class AIAssessmentNotFoundException : MameyException
{
    public AIAssessmentNotFoundException(AIAssessmentId aiassessmentId)
        : base($"AIAssessment with ID: '{aiassessmentId.Value}' was not found.")
        => AIAssessmentId = aiassessmentId;

    public AIAssessmentId AIAssessmentId { get; }
}

