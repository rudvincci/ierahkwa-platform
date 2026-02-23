namespace FraudDetection.Core.Models;

public class FraudAlert
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TransactionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public AlertType Type { get; set; }
    public RiskLevel Risk { get; set; }
    public decimal Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Indicators { get; set; } = new();
    public AlertStatus Status { get; set; } = AlertStatus.Open;
    public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? Resolution { get; set; }
}

public enum AlertType { UnusualActivity, HighValue, Velocity, Geographic, DeviceAnomaly, BehaviorChange }
public enum RiskLevel { Low, Medium, High, Critical }
public enum AlertStatus { Open, UnderReview, Resolved, FalsePositive, Escalated }

public class FraudStats
{
    public int TotalAlerts { get; set; }
    public int OpenAlerts { get; set; }
    public int BlockedTransactions { get; set; }
    public decimal AmountProtected { get; set; }
    public decimal DetectionRate { get; set; }
}
