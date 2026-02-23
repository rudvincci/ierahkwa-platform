using Mamey.CQRS;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Events;

public record TenantCreated(
    TenantId TenantId,
    string DisplayName) : IDomainEvent;
