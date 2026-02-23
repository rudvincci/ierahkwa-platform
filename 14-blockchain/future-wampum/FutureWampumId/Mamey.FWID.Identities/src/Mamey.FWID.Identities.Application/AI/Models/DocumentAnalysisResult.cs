namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// Result of AI document analysis.
/// </summary>
public class DocumentAnalysisResult
{
    public Guid AnalysisId { get; set; } = Guid.NewGuid();
    public string DocumentType { get; set; } = null!;
    public DocumentClassification Classification { get; set; }
    public double AuthenticityScore { get; set; }
    public double QualityScore { get; set; }
    public bool TamperingDetected { get; set; }
    public List<TamperingIndicator> TamperingIndicators { get; set; } = new();
    public ExtractedDocumentData? ExtractedData { get; set; }
    public List<DocumentAnomaly> Anomalies { get; set; } = new();
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

public enum DocumentClassification
{
    Passport = 1,
    NationalId = 2,
    DriversLicense = 3,
    TribalId = 4,
    BirthCertificate = 5,
    ProofOfAddress = 6,
    Unknown = 99
}

public class TamperingIndicator
{
    public string Type { get; set; } = null!;
    public string Location { get; set; } = null!;
    public double Confidence { get; set; }
    public string Description { get; set; } = null!;
}

public class ExtractedDocumentData
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? DocumentNumber { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? IssuingAuthority { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    public string? MRZLine1 { get; set; }
    public string? MRZLine2 { get; set; }
    public Dictionary<string, string> AdditionalFields { get; set; } = new();
    public Dictionary<string, double> FieldConfidence { get; set; } = new();
}

public class DocumentAnomaly
{
    public string AnomalyType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Severity { get; set; }
    public string? ExpectedValue { get; set; }
    public string? ActualValue { get; set; }
}
