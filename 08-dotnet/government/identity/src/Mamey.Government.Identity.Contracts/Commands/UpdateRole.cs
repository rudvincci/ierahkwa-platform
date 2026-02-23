using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record UpdateRole : ICommand
{
    public UpdateRole(Guid id, string name, string description, IEnumerable<Guid>? permissionIds = null)
    {
        Id = id;
        Name = name;
        Description = description;
        PermissionIds = permissionIds ?? Enumerable.Empty<Guid>();
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public IEnumerable<Guid> PermissionIds { get; init; }
}
