namespace Mamey.Blockchain.Compliance;

public class CheckAMLResult
{
    public bool Flagged { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public List<string> RiskFactors { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class VerifyKYCResult
{
    public bool Verified { get; set; }
    public string KycLevel { get; set; } = string.Empty;
    public List<string> VerifiedAttributes { get; set; } = new();
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}




