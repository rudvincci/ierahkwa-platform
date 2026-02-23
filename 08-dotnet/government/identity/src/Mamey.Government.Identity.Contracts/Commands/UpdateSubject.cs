using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record UpdateSubject : ICommand
{
    public UpdateSubject(Guid id, string name, string email, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Email = email;
        Tags = tags;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public IEnumerable<string> Tags { get; init; }
}


