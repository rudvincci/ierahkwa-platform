using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.CMS.Core.Events;

public record ContentPublishedEvent(Guid ContentId, DateTime PublishedAt) : IEvent;
