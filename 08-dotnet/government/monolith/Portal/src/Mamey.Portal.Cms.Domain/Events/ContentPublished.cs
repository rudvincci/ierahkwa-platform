using Mamey.CQRS;

namespace Mamey.Portal.Cms.Domain.Events;

public sealed record ContentPublished(
    Guid ContentId,
    string ContentType,
    string TenantId,
    DateTimeOffset PublishedAt) : IDomainEvent;
