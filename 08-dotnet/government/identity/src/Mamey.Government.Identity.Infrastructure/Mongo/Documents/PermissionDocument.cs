using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class PermissionDocument : IIdentifiable<Guid>
{
    public PermissionDocument()
    {

    }

    public PermissionDocument(Permission permission)
    {
        if (permission is null)
        {
            throw new NullReferenceException();
        }

        Id = permission.Id.Value;
        Name = permission.Name;
        Resource = permission.Resource;
        Action = permission.Action;
        Status = permission.Status.ToString();
        CreatedAt = permission.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = permission.ModifiedAt?.ToUnixTimeMilliseconds();
        Version = permission.Version;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public string Status { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public int Version { get; set; }

    public Permission AsEntity()
        => new(Id, Name, "", Resource, Action,
               CreatedAt.GetDate(), ModifiedAt?.GetDate(),
               Status.ToEnum<PermissionStatus>(), Version);

    public PermissionDto AsDto()
        => new PermissionDto(Id, Name, "", Resource, Action, Status, CreatedAt.GetDate(), ModifiedAt?.GetDate());
    
}
