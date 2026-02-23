using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class SubjectDetailsDto : SubjectDto
{
    public SubjectDetailsDto(Guid id, string name, IEnumerable<string> tags, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    
    public SubjectDetailsDto(Guid id, string name, string email, string status, IEnumerable<string> tags, IEnumerable<Guid> roles, DateTime? lastAuthenticatedAt, DateTime createdAt, DateTime? modifiedAt)
        : base(id, name, tags)
    {
        Email = email;
        Status = status;
        Roles = roles;
        LastAuthenticatedAt = lastAuthenticatedAt;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    
    public SubjectDetailsDto(SubjectDto subjectDto, DateTime createdAt, DateTime? modifiedAt)
        : base(subjectDto.Id, subjectDto.Name, subjectDto.Tags)
    {
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
    
    public string? Email { get; set; }
    public string? Status { get; set; }
    public IEnumerable<Guid>? Roles { get; set; }
    public DateTime? LastAuthenticatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}









