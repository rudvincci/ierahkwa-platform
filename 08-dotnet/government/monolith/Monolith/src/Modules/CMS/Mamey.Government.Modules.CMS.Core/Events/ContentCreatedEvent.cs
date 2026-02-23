using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.CMS.Core.Events;

public record ContentCreatedEvent(Guid ContentId, Guid TenantId, string Title, string Slug) : IEvent;
