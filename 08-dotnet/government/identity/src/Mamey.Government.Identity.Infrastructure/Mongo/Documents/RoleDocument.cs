using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class RoleDocument : IIdentifiable<Guid>
{
    public RoleDocument()
    {

    }

    public RoleDocument(Role role)
    {
        if (role is null)
        {
            throw new NullReferenceException();
        }

        Id = role.Id.Value;
        Name = role.Name;
        Description = role.Description;
        Permissions = role.Permissions.Select(p => p.Value).ToList();
        Status = role.Status.ToString();
        CreatedAt = role.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = role.ModifiedAt?.ToUnixTimeMilliseconds();
        Version = role.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Guid> Permissions { get; set; }
    public string Status { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public int Version { get; set; }

    public Role AsEntity() 
        => new(Id, Name, Description, CreatedAt.GetDate(),
               ModifiedAt?.GetDate(), Permissions.Select(p => new PermissionId(p)),
               Status.ToEnum<RoleStatus>(), Version);

    public RoleDto AsDto()
        => new RoleDto(Id, Name, Description, Status, Permissions, CreatedAt.GetDate(), ModifiedAt?.GetDate());
    
}
