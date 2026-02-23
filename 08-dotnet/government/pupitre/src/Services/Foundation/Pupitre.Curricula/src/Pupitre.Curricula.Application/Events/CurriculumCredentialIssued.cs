using Mamey.CQRS.Events;

namespace Pupitre.Curricula.Application.Events;

internal record CurriculumCredentialIssued(Guid CurriculumId, string IdentityId, string? LedgerTransactionId) : IEvent;
