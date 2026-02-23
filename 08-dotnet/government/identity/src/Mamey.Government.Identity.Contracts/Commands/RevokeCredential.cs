using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record RevokeCredential : ICommand
{
    public RevokeCredential(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}
