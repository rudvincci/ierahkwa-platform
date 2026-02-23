using Mamey.CQRS;
using Mamey.Government.Modules.Tenant.Core.Domain.Entities;

namespace Mamey.Government.Modules.Tenant.Core.Domain.Events;

public record TenantModified(TenantEntity Tenant) : IDomainEvent;
