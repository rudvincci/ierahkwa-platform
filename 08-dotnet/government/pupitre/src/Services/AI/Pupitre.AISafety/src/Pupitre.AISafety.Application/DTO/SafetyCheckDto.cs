namespace Pupitre.AISafety.Application.DTO;

public class SafetyCheckDto
{
    public SafetyCheckDto(Guid id, string name, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Tags = tags;
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Tags { get; set; }
}
