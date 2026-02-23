namespace Mamey.Portal.Citizenship.Application.Models;

public sealed record ApplicationStatusDto(
    Guid Id,
    string ApplicationNumber,
    string Status,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string? Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ApplicationStatusUploadDto> Uploads,
    IReadOnlyList<ApplicationStatusDocumentDto> IssuedDocuments,
    IReadOnlyList<ApplicationStatusTimelineEntryDto> Timeline,
    ApplicationStatusIntakeReviewDto? IntakeReview = null,
    string? RejectionReason = null);

public sealed record ApplicationStatusUploadDto(
    Guid Id,
    string Kind,
    string FileName,
    string ContentType,
    long Size,
    DateTimeOffset UploadedAt);

public sealed record ApplicationStatusDocumentDto(
    Guid Id,
    string Kind,
    string? DocumentNumber,
    string FileName,
    DateTimeOffset? ExpiresAt,
    DateTimeOffset CreatedAt);

public sealed record ApplicationStatusTimelineEntryDto(
    DateTimeOffset Timestamp,
    string Event,
    string Description);

public sealed record ApplicationStatusIntakeReviewDto(
    string ReviewerName,
    DateTime ReviewDate,
    string Recommendation,
    string? RecommendationReason,
    bool ApplicationComplete,
    bool AllDocumentsReceived,
    bool IdentityVerified,
    DateTimeOffset CreatedAt);


