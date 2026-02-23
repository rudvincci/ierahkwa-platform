using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record UpdateUser : ICommand
{
    public UpdateUser(Guid id, string username, string email)
    {
        Id = id;
        Username = username;
        Email = email;
    }

    public Guid Id { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
}
