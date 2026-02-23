namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

/// <summary>
/// Stores intake review data (CIT-001-F) for citizenship applications
/// </summary>
public sealed class IntakeReviewRow
{
    public Guid Id { get; set; }

    public string TenantId { get; set; } = string.Empty;

    public Guid ApplicationId { get; set; }

    /// <summary>
    /// Name of the reviewer/agent
    /// </summary>
    public string ReviewerName { get; set; } = string.Empty;

    /// <summary>
    /// Date of the review
    /// </summary>
    public DateTime ReviewDate { get; set; }

    // Application Completeness
    public bool ApplicationComplete { get; set; }
    public bool AllDocumentsReceived { get; set; }
    public bool IdentityVerified { get; set; }
    public bool BackgroundCheckComplete { get; set; }

    // Document Verification
    public bool BirthCertificateVerified { get; set; }
    public bool PhotoIdVerified { get; set; }
    public bool ProofOfResidenceVerified { get; set; }
    public bool PassportPhotoVerified { get; set; }
    public bool SignatureVerified { get; set; }

    // Review Notes
    public string CompletenessNotes { get; set; } = string.Empty;
    public string DocumentNotes { get; set; } = string.Empty;
    public string AdditionalNotes { get; set; } = string.Empty;

    // Recommendation
    public string Recommendation { get; set; } = string.Empty; // "Approve", "Reject", "RequestAdditionalInfo", "Pending"
    public string RecommendationReason { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Navigation property to the application
    /// </summary>
    public CitizenshipApplicationRow? Application { get; set; }
}

