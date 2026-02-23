namespace Mamey.AI.Government.Models;

public class FraudScore
{
    public double Score { get; set; } // 0-100
    public string RiskLevel { get; set; } = "Unknown"; // Low, Medium, High, Critical
    public List<FraudIndicator> Indicators { get; set; } = new();
    public string Recommendation { get; set; } = string.Empty;
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
