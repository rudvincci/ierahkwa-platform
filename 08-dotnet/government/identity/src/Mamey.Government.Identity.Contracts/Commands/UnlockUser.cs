using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record UnlockUser : ICommand
{
    public UnlockUser(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; init; }
}
