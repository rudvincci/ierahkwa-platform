using Mamey.CQRS.Events;

namespace Pupitre.Ministries.Application.Events;

internal record MinistryCredentialIssued(Guid MinistryDataId, string IdentityId, string? LedgerTransactionId) : IEvent;
