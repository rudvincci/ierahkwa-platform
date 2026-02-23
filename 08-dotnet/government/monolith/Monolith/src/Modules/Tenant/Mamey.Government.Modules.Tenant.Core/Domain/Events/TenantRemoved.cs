using Mamey.CQRS;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Events;

public record TenantRemoved(TenantId TenantId) : IDomainEvent;
