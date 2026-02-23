namespace Mamey.AI.Identity.Models;

/// <summary>
/// Represents a comprehensive risk assessment for an identity operation.
/// </summary>
public class RiskAssessment
{
    /// <summary>
    /// Overall risk score (0-100, where higher = higher risk).
    /// </summary>
    public double OverallRiskScore { get; set; }

    /// <summary>
    /// Risk level: Low, Medium, High, Critical.
    /// </summary>
    public string RiskLevel { get; set; } = "Unknown";

    /// <summary>
    /// Breakdown of risk scores by category.
    /// </summary>
    public Dictionary<string, double> CategoryScores { get; set; } = new();

    /// <summary>
    /// List of risk factors identified.
    /// </summary>
    public List<RiskFactor> RiskFactors { get; set; } = new();

    /// <summary>
    /// Recommended action based on risk assessment.
    /// </summary>
    public string RecommendedAction { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the assessment was calculated.
    /// </summary>
    public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a single risk factor contributing to the overall risk score.
/// </summary>
public class RiskFactor
{
    /// <summary>
    /// Category of the risk factor (e.g., "Behavioral", "Pattern", "Temporal").
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Description of the risk factor.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Contribution to the overall risk score (0-100).
    /// </summary>
    public double Contribution { get; set; }

    /// <summary>
    /// Severity: Low, Medium, High, Critical.
    /// </summary>
    public string Severity { get; set; } = "Medium";
}

/// <summary>
/// Provides explainability for a risk assessment.
/// </summary>
public class RiskExplanation
{
    /// <summary>
    /// Human-readable explanation of the risk assessment.
    /// </summary>
    public string Explanation { get; set; } = string.Empty;

    /// <summary>
    /// Top contributing factors to the risk score.
    /// </summary>
    public List<RiskFactor> TopFactors { get; set; } = new();

    /// <summary>
    /// Suggested mitigation strategies.
    /// </summary>
    public List<string> MitigationStrategies { get; set; } = new();
}
