using Mamey.CQRS;
using Pupitre.Assessments.Domain.Entities;

namespace Pupitre.Assessments.Domain.Events;

internal record AssessmentRemoved(Assessment Assessment) : IDomainEvent;