using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Tenant.Core.Commands;

public record ActivateTenant(Guid TenantId, string ActivatedBy) : ICommand;
