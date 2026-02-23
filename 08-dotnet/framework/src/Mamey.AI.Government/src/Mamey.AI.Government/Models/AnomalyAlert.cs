namespace Mamey.AI.Government.Models;

public class AnomalyAlert
{
    public string AlertId { get; set; } = Guid.NewGuid().ToString();
    public string AlertType { get; set; } = "General";
    public double Score { get; set; }
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}
