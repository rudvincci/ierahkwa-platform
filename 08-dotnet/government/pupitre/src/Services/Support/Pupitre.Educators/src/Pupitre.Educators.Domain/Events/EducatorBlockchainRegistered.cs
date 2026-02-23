using Mamey.CQRS;

namespace Pupitre.Educators.Domain.Events;

internal record EducatorBlockchainRegistered(Guid EducatorId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
