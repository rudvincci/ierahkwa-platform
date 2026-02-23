using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Tenant.Core.Commands;

public record CreateTenant(
    string DisplayName,
    string? Domain = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
