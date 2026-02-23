using Mamey.CQRS.Events;

namespace Pupitre.Users.Application.Events;

internal record UserCredentialIssued(Guid UserId, string IdentityId, string? LedgerTransactionId) : IEvent;
