using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record ChangeUserPassword : ICommand
{
    public ChangeUserPassword(Guid userId, string newPasswordHash)
    {
        UserId = userId;
        NewPasswordHash = newPasswordHash;
    }

    public Guid UserId { get; init; }
    public string NewPasswordHash { get; init; }
}
