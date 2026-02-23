using System;
using Mamey.CQRS.Events;

namespace Mamey.Government.Modules.Tenant.Core.Events;

public record TenantCreatedEvent(Guid TenantId, string DisplayName) : IEvent;
