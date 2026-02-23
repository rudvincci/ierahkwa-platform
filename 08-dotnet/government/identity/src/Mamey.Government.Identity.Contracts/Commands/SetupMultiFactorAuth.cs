using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record SetupMultiFactorAuth : ICommand
{
    public SetupMultiFactorAuth(Guid id, Guid userId, IEnumerable<int>? enabledMethods = null, int requiredMethods = 2)
    {
        Id = id;
        UserId = userId;
        EnabledMethods = enabledMethods ?? Enumerable.Empty<int>();
        RequiredMethods = requiredMethods;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public IEnumerable<int> EnabledMethods { get; init; }
    public int RequiredMethods { get; init; }
}
