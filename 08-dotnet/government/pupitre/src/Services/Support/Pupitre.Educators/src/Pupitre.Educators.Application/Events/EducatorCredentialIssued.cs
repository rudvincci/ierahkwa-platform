using Mamey.CQRS.Events;

namespace Pupitre.Educators.Application.Events;

internal record EducatorCredentialIssued(Guid EducatorId, string IdentityId, string? LedgerTransactionId) : IEvent;
