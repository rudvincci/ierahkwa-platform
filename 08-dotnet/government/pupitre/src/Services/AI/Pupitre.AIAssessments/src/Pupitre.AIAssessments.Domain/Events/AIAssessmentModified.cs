using Mamey.CQRS;
using Pupitre.AIAssessments.Domain.Entities;

namespace Pupitre.AIAssessments.Domain.Events;

internal record AIAssessmentModified(AIAssessment AIAssessment): IDomainEvent;

