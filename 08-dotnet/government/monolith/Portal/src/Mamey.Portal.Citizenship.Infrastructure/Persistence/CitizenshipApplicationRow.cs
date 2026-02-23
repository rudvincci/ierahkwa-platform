namespace Mamey.Portal.Citizenship.Infrastructure.Persistence;

public sealed class CitizenshipApplicationRow
{
    public Guid Id { get; set; }

    public string TenantId { get; set; } = string.Empty;
    public string ApplicationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Submitted";

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string? Email { get; set; }

    // AAMVA-compliant fields
    public string? MiddleName { get; set; }
    public string? Height { get; set; }
    public string? EyeColor { get; set; }
    public string? HairColor { get; set; }
    public string? Sex { get; set; }

    // Additional fields
    public string? PhoneNumber { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? MaritalStatus { get; set; }
    public string? PreviousNames { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }

    // Form acknowledgments and consents
    public bool AcknowledgeTreaty { get; set; }
    public bool SwearAllegiance { get; set; }
    public DateTime? AffidavitDate { get; set; }
    public bool HasBirthCertificate { get; set; }
    public bool HasPhotoId { get; set; }
    public bool HasProofOfResidence { get; set; }
    public bool HasBackgroundCheck { get; set; }
    public bool AuthorizeBiometricEnrollment { get; set; }
    public bool DeclareUnderstanding { get; set; }
    public bool ConsentToVerification { get; set; }
    public bool ConsentToDataProcessing { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? RejectionReason { get; set; }

    // Extended application data stored as JSON for additional fields
    // This includes: emergency contact, employment, education, family info, 
    // criminal history, travel history, references, additional ID, previous addresses
    public string? ExtendedDataJson { get; set; }

    public List<CitizenshipUploadRow> Uploads { get; set; } = new();

    public List<CitizenshipIssuedDocumentRow> IssuedDocuments { get; set; } = new();
}


