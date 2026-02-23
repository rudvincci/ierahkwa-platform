using Mamey.CQRS;

namespace Pupitre.Notifications.Domain.Events;

internal record NotificationBlockchainRegistered(Guid NotificationId, string IdentityId, string? LedgerTransactionId) : IDomainEvent;
