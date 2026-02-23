using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Documents.Core.Events;

public record DocumentDeletedEvent(Guid DocumentId, Guid TenantId) : IEvent;
