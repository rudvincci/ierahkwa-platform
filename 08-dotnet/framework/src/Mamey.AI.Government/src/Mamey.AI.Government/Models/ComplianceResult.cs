namespace Mamey.AI.Government.Models;

public class ComplianceResult
{
    public bool IsCompliant { get; set; }
    public List<string> Violations { get; set; } = new();
    public string RegulationId { get; set; } = string.Empty;
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}
