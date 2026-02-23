using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record DisableMfaMethod : ICommand
{
    public DisableMfaMethod(Guid userId, int method)
    {
        UserId = userId;
        Method = method;
    }

    public Guid UserId { get; init; }
    public int Method { get; init; }
}
