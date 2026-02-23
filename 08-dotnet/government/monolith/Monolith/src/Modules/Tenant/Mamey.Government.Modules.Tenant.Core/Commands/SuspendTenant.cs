using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Tenant.Core.Commands;

public record SuspendTenant(Guid TenantId, string SuspendedBy) : ICommand;
