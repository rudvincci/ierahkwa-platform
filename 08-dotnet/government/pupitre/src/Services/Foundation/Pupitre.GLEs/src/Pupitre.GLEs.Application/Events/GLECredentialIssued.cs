using Mamey.CQRS.Events;

namespace Pupitre.GLEs.Application.Events;

internal record GLECredentialIssued(Guid GLEId, string IdentityId, string? LedgerTransactionId) : IEvent;
