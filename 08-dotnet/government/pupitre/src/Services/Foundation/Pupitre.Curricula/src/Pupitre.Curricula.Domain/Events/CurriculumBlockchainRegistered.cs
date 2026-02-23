using Mamey.CQRS;

namespace Pupitre.Curricula.Domain.Events;

internal record CurriculumBlockchainRegistered(Guid CurriculumId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
