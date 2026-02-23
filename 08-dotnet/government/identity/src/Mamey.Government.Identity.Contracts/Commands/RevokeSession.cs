using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record RevokeSession : ICommand
{
    public RevokeSession(Guid sessionId)
    {
        SessionId = sessionId;
    }

    public Guid SessionId { get; init; }
}
