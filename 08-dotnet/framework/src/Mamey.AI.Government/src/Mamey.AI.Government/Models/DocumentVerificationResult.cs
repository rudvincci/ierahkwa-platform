namespace Mamey.AI.Government.Models;

public class DocumentVerificationResult
{
    public bool IsVerified { get; set; }
    public double ConfidenceScore { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public bool IsAuthentic { get; set; }
    public double AuthenticityScore { get; set; }
    public Dictionary<string, string> ExtractedData { get; set; } = new();
    public List<string> ValidationErrors { get; set; } = new();
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;

    public static DocumentVerificationResult Success(double confidence, string docType, Dictionary<string, string> data)
    {
        return new DocumentVerificationResult
        {
            IsVerified = true,
            ConfidenceScore = confidence,
            DocumentType = docType,
            IsAuthentic = true,
            AuthenticityScore = 1.0, // Placeholder
            ExtractedData = data
        };
    }

    public static DocumentVerificationResult Failed(string error)
    {
        return new DocumentVerificationResult
        {
            IsVerified = false,
            ValidationErrors = new List<string> { error }
        };
    }
}
