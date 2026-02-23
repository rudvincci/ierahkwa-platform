using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class PermissionDto
{
    public PermissionDto(Guid id, string name, string description, string resource, string action, string status, DateTime createdAt, DateTime? modifiedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        Resource = resource;
        Action = action;
        Status = status;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public string Status { get; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}










