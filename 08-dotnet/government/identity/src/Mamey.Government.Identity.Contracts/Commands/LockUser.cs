using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record LockUser : ICommand
{
    public LockUser(Guid userId, DateTime lockedUntil)
    {
        UserId = userId;
        LockedUntil = lockedUntil;
    }

    public Guid UserId { get; init; }
    public DateTime LockedUntil { get; init; }
}
