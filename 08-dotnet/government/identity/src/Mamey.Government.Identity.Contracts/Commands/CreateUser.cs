using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record CreateUser : ICommand
{
    public CreateUser(Guid id, Guid subjectId, string username, string email, string passwordHash)
    {
        Id = id;
        SubjectId = subjectId;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SubjectId { get; init; }
    public string Username { get; init; }
    public string Email { get; init; }
    public string PasswordHash { get; init; }
}
