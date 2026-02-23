using System;
using Mamey.CQRS.Commands;

namespace Mamey.Government.Modules.Passports.Core.Commands;

public record IssuePassport(
    Guid CitizenId,
    Guid TenantId,
    string PassportType = "Standard",
    int ValidityYears = 10) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
