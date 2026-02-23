namespace Mamey.Portal.Citizenship.Application.Services;

public interface IApplicationSubmissionStore
{
    Task SaveAsync(ApplicationSubmission submission, IReadOnlyList<ApplicationUploadRecord> uploads, CancellationToken ct = default);
}

public sealed record ApplicationSubmission(
    Guid Id,
    string TenantId,
    string ApplicationNumber,
    string Status,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string? MiddleName,
    string? Height,
    string? EyeColor,
    string? HairColor,
    string? Sex,
    string? PhoneNumber,
    string? PlaceOfBirth,
    string? CountryOfOrigin,
    string? MaritalStatus,
    string? PreviousNames,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode,
    bool AcknowledgeTreaty,
    bool SwearAllegiance,
    DateTime? AffidavitDate,
    bool HasBirthCertificate,
    bool HasPhotoId,
    bool HasProofOfResidence,
    bool HasBackgroundCheck,
    bool AuthorizeBiometricEnrollment,
    bool DeclareUnderstanding,
    bool ConsentToVerification,
    bool ConsentToDataProcessing,
    string? ExtendedDataJson,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ApplicationUploadRecord(
    Guid Id,
    Guid ApplicationId,
    string Kind,
    string FileName,
    string ContentType,
    long Size,
    string StorageBucket,
    string StorageKey,
    DateTimeOffset UploadedAt);
