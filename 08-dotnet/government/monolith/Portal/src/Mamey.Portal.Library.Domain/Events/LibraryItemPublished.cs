using Mamey.CQRS;

namespace Mamey.Portal.Library.Domain.Events;

public sealed record LibraryItemPublished(
    Guid LibraryItemId,
    string TenantId,
    DateTimeOffset PublishedAt) : IDomainEvent;
