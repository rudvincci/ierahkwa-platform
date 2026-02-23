using Mamey.CQRS;

namespace Pupitre.Assessments.Domain.Events;

internal record AssessmentBlockchainRegistered(Guid AssessmentId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
