namespace Mamey.Portal.Citizenship.Domain.Entities;

public sealed class IntakeReview
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; } = string.Empty;
    public Guid ApplicationId { get; private set; }
    public string ReviewerName { get; private set; } = string.Empty;
    public DateTime ReviewDate { get; private set; }

    public bool ApplicationComplete { get; private set; }
    public bool AllDocumentsReceived { get; private set; }
    public bool IdentityVerified { get; private set; }
    public bool BackgroundCheckComplete { get; private set; }
    public bool BirthCertificateVerified { get; private set; }
    public bool PhotoIdVerified { get; private set; }
    public bool ProofOfResidenceVerified { get; private set; }
    public bool PassportPhotoVerified { get; private set; }
    public bool SignatureVerified { get; private set; }

    public string CompletenessNotes { get; private set; } = string.Empty;
    public string DocumentNotes { get; private set; } = string.Empty;
    public string AdditionalNotes { get; private set; } = string.Empty;

    public string Recommendation { get; private set; } = string.Empty;
    public string RecommendationReason { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private IntakeReview() { }

    public IntakeReview(
        Guid id,
        string tenantId,
        Guid applicationId,
        string reviewerName,
        DateTime reviewDate,
        bool applicationComplete,
        bool allDocumentsReceived,
        bool identityVerified,
        bool backgroundCheckComplete,
        bool birthCertificateVerified,
        bool photoIdVerified,
        bool proofOfResidenceVerified,
        bool passportPhotoVerified,
        bool signatureVerified,
        string completenessNotes,
        string documentNotes,
        string additionalNotes,
        string recommendation,
        string recommendationReason,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        TenantId = tenantId;
        ApplicationId = applicationId;
        ReviewerName = reviewerName;
        ReviewDate = reviewDate;
        ApplicationComplete = applicationComplete;
        AllDocumentsReceived = allDocumentsReceived;
        IdentityVerified = identityVerified;
        BackgroundCheckComplete = backgroundCheckComplete;
        BirthCertificateVerified = birthCertificateVerified;
        PhotoIdVerified = photoIdVerified;
        ProofOfResidenceVerified = proofOfResidenceVerified;
        PassportPhotoVerified = passportPhotoVerified;
        SignatureVerified = signatureVerified;
        CompletenessNotes = completenessNotes;
        DocumentNotes = documentNotes;
        AdditionalNotes = additionalNotes;
        Recommendation = recommendation;
        RecommendationReason = recommendationReason;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
