namespace Mamey.Portal.Citizenship.Application.Requests;

public sealed record SubmitCitizenshipApplicationRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Email,
    string? AddressLine1,
    string? City,
    string? Region,
    string? PostalCode,
    // AAMVA-compliant fields
    string? MiddleName = null,
    string? Height = null,
    string? EyeColor = null,
    string? HairColor = null,
    string? Sex = null,
    // Additional fields
    string? PhoneNumber = null,
    string? PlaceOfBirth = null,
    string? CountryOfOrigin = null,
    string? MaritalStatus = null,
    string? PreviousNames = null,
    // Form acknowledgments and consents
    bool AcknowledgeTreaty = false,
    bool SwearAllegiance = false,
    DateTime? AffidavitDate = null,
    bool HasBirthCertificate = false,
    bool HasPhotoId = false,
    bool HasProofOfResidence = false,
    bool HasBackgroundCheck = false,
    bool AuthorizeBiometricEnrollment = false,
    bool DeclareUnderstanding = false,
    bool ConsentToVerification = false,
    bool ConsentToDataProcessing = false,
    // Extended application data (stored as JSON)
    string? ExtendedDataJson = null);




