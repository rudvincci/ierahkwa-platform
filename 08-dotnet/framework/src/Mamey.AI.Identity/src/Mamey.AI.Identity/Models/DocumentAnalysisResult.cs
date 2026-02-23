namespace Mamey.AI.Identity.Models;

/// <summary>
/// Represents the result of document analysis.
/// </summary>
public class DocumentAnalysisResult
{
    /// <summary>
    /// Extracted text from the document.
    /// </summary>
    public Dictionary<string, string> ExtractedFields { get; set; } = new();

    /// <summary>
    /// Confidence scores for each extracted field (0-1.0).
    /// </summary>
    public Dictionary<string, double> FieldConfidences { get; set; } = new();

    /// <summary>
    /// Whether the document appears to be authentic.
    /// </summary>
    public bool IsAuthentic { get; set; }

    /// <summary>
    /// Authenticity score (0-1.0).
    /// </summary>
    public double AuthenticityScore { get; set; }

    /// <summary>
    /// List of detected tampering indicators.
    /// </summary>
    public List<string> TamperingIndicators { get; set; } = new();

    /// <summary>
    /// Timestamp when the analysis was performed.
    /// </summary>
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents the result of document verification.
/// </summary>
public class DocumentVerificationResult
{
    /// <summary>
    /// Whether the document is verified as authentic.
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Verification confidence (0-1.0).
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// List of verification checks performed.
    /// </summary>
    public List<VerificationCheck> Checks { get; set; } = new();

    /// <summary>
    /// Overall verification status.
    /// </summary>
    public string Status { get; set; } = "Unknown";

    /// <summary>
    /// Timestamp when verification was performed.
    /// </summary>
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a single verification check.
/// </summary>
public class VerificationCheck
{
    /// <summary>
    /// Name of the check (e.g., "Hologram", "Microprint", "UV").
    /// </summary>
    public string CheckName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the check passed.
    /// </summary>
    public bool Passed { get; set; }

    /// <summary>
    /// Confidence in the check result (0-1.0).
    /// </summary>
    public double Confidence { get; set; }
}
