using Mamey.CQRS.Events;

namespace Pupitre.Assessments.Application.Events;

internal record AssessmentCredentialIssued(Guid AssessmentId, string IdentityId, string? LedgerTransactionId) : IEvent;
