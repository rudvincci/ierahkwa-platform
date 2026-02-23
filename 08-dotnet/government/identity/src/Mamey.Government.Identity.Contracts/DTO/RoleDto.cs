using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class RoleDto
{
    public RoleDto(Guid id, string name, string description, string status, IEnumerable<Guid> permissionIds, DateTime createdAt, DateTime? modifiedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        Status = status;
        PermissionIds = permissionIds;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public IEnumerable<Guid> PermissionIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}










