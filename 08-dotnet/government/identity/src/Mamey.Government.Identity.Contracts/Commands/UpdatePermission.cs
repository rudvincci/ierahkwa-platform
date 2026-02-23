using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record UpdatePermission : ICommand
{
    public UpdatePermission(Guid id, string name, string description, string resource, string action)
    {
        Id = id;
        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Resource { get; init; }
    public string Action { get; init; }
}
