using Mamey.CQRS;

namespace Pupitre.Ministries.Domain.Events;

internal record MinistryDataBlockchainRegistered(Guid MinistryDataId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
