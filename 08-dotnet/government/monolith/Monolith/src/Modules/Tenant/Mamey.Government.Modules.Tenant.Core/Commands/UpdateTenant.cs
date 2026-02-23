using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Tenant.Core.Commands;

public record UpdateTenant(
    Guid TenantId,
    string DisplayName,
    string? Domain = null) : ICommand;
