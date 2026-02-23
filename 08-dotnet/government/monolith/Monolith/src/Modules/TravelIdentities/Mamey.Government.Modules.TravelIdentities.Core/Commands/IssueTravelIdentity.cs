using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.TravelIdentities.Core.Commands;

public record IssueTravelIdentity(
    Guid CitizenId,
    Guid TenantId,
    int ValidityYears = 8) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
