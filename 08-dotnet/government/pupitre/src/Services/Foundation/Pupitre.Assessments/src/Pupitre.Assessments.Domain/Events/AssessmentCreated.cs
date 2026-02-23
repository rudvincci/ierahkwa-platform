using Mamey.CQRS;
using Pupitre.Assessments.Domain.Entities;

namespace Pupitre.Assessments.Domain.Events;

internal record AssessmentCreated(Assessment Assessment) : IDomainEvent;

