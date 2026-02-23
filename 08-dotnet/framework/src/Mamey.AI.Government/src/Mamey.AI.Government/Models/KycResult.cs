namespace Mamey.AI.Government.Models;

public class KycResult
{
    public bool IsApproved { get; set; }
    public string RiskLevel { get; set; } = "Low"; // Low, Medium, High
    public double RiskScore { get; set; }
    public List<string> Flags { get; set; } = new();
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public SanctionsCheckResult SanctionsResult { get; set; } = new();
    public PepCheckResult PepResult { get; set; } = new();
}

public class SanctionsCheckResult
{
    public bool IsClean { get; set; } = true;
    public List<string> MatchedLists { get; set; } = new();
}

public class PepCheckResult
{
    public bool IsPep { get; set; } = false;
    public string Details { get; set; } = string.Empty;
}
