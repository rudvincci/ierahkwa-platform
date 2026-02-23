using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core.DTO;

namespace Mamey.Government.Modules.Tenant.Core.Queries;

internal class GetTenant : IQuery<TenantDto?>
{
    public Guid TenantId { get; set; }
}
