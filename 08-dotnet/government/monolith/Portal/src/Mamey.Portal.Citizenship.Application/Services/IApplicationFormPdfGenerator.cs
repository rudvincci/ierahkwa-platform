namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Generates PDF versions of completed citizenship application forms
/// </summary>
public interface IApplicationFormPdfGenerator
{
    /// <summary>
    /// Generates PDF for CIT-001-A (Main Application Form)
    /// </summary>
    Task<byte[]> GenerateCit001AAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-B (Treaty Acknowledgment)
    /// </summary>
    Task<byte[]> GenerateCit001BAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-C (Affidavit of Allegiance)
    /// </summary>
    Task<byte[]> GenerateCit001CAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-D (Supporting Document Checklist)
    /// </summary>
    Task<byte[]> GenerateCit001DAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-E (Biometric Enrollment Authorization)
    /// </summary>
    Task<byte[]> GenerateCit001EAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-G (Declaration of Understanding)
    /// </summary>
    Task<byte[]> GenerateCit001GAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates PDF for CIT-001-H (Consent to Verification and Data Processing)
    /// </summary>
    Task<byte[]> GenerateCit001HAsync(ApplicationFormData data, CancellationToken ct = default);

    /// <summary>
    /// Generates all form PDFs for an application
    /// </summary>
    Task<Dictionary<string, byte[]>> GenerateAllFormsAsync(ApplicationFormData data, CancellationToken ct = default);
}

/// <summary>
/// Data structure containing all application form information for PDF generation
/// </summary>
public sealed record ApplicationFormData(
    string ApplicationNumber,
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly DateOfBirth,
    string? PlaceOfBirth,
    string? CountryOfOrigin,
    string? Sex,
    string? Height,
    string? EyeColor,
    string? HairColor,
    string? MaritalStatus,
    string? PreviousNames,
    string Email,
    string? PhoneNumber,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode,
    // Form acknowledgments
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
    DateTimeOffset SubmittedAt);

