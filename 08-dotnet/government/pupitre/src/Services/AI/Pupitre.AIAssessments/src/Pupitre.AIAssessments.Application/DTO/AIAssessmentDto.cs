namespace Pupitre.AIAssessments.Application.DTO;

public class AIAssessmentDto
{
    public AIAssessmentDto(Guid id, string name, IEnumerable<string> tags)
    {
        Id = id;
        Name = name;
        Tags = tags;
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Tags { get; set; }
}
