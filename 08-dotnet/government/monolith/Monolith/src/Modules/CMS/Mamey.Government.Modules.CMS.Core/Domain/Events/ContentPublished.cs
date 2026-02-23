using Mamey.CQRS;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.CMS.Core.Domain.Events;

public record ContentPublished(
    ContentId ContentId,
    DateTime PublishedAt) : IDomainEvent;
