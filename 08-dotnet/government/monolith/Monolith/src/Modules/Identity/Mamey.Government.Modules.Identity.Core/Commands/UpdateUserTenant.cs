using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Identity.Core.Commands;

public record UpdateUserTenant(Guid UserId, Guid? TenantId) : ICommand;
