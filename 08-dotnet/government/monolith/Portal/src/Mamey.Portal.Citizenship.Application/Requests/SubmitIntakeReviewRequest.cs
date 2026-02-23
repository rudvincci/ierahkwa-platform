namespace Mamey.Portal.Citizenship.Application.Requests;

public sealed record SubmitIntakeReviewRequest(
    Guid ApplicationId,
    string ReviewerName,
    DateTime ReviewDate,
    // Application Completeness
    bool ApplicationComplete,
    bool AllDocumentsReceived,
    bool IdentityVerified,
    bool BackgroundCheckComplete,
    // Document Verification
    bool BirthCertificateVerified,
    bool PhotoIdVerified,
    bool ProofOfResidenceVerified,
    bool PassportPhotoVerified,
    bool SignatureVerified,
    // Review Notes
    string? CompletenessNotes = null,
    string? DocumentNotes = null,
    string? AdditionalNotes = null,
    // Recommendation
    string Recommendation = "Pending",
    string? RecommendationReason = null);

