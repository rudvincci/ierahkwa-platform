namespace Mamey.AI.Identity.Models;

/// <summary>
/// Represents a fraud score with indicators and recommendations.
/// </summary>
public class FraudScore
{
    /// <summary>
    /// Fraud score from 0-100 (higher = more suspicious).
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Risk level: Low, Medium, High, Critical.
    /// </summary>
    public string RiskLevel { get; set; } = "Unknown";

    /// <summary>
    /// List of fraud indicators detected.
    /// </summary>
    public List<FraudIndicator> Indicators { get; set; } = new();

    /// <summary>
    /// Recommendation based on the fraud score.
    /// </summary>
    public string Recommendation { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the score was calculated.
    /// </summary>
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a single fraud indicator.
/// </summary>
public class FraudIndicator
{
    /// <summary>
    /// Type of fraud indicator (e.g., "DuplicateIdentity", "SuspiciousPattern").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Description of the indicator.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Confidence level (0-1.0).
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Severity: Low, Medium, High, Critical.
    /// </summary>
    public string Severity { get; set; } = "Medium";
}
