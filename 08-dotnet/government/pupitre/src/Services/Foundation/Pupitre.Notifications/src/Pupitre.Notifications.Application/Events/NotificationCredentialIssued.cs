using Mamey.CQRS.Events;

namespace Pupitre.Notifications.Application.Events;

internal record NotificationCredentialIssued(Guid NotificationId, string IdentityId, string? LedgerTransactionId) : IEvent;
