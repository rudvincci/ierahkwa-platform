using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.DTO;

public class SubjectDto
{
    public SubjectDto(Guid id, string name, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Tags = tags;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Tags { get; set; }
}









