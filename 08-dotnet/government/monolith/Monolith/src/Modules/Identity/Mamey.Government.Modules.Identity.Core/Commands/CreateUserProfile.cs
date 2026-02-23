using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Identity.Core.Commands;

public record CreateUserProfile(
    string AuthenticatorIssuer,
    string AuthenticatorSubject,
    string? Email = null,
    string? DisplayName = null,
    Guid? TenantId = null) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
