using Mamey.CQRS;

namespace Mamey.Portal.Cms.Domain.Events;

public sealed record ContentRejected(
    Guid ContentId,
    string ContentType,
    string TenantId,
    string Reason,
    DateTimeOffset RejectedAt) : IDomainEvent;
