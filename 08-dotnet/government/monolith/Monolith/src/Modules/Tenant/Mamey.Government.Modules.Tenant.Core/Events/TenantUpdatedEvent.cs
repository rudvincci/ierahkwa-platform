using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Tenant.Core.Events;

public record TenantUpdatedEvent(Guid TenantId, string DisplayName) : IEvent;
