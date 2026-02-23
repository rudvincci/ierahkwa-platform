namespace Mamey.FWID.Identities.Contracts.DTOs;

/// <summary>
/// Data transfer object for personal details.
/// </summary>
public class PersonalDetailsDto
{
    /// <summary>
    /// The date of birth.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// The place of birth.
    /// </summary>
    public string? PlaceOfBirth { get; set; }

    /// <summary>
    /// The gender.
    /// </summary>
    public string? Gender { get; set; }

    /// <summary>
    /// The clan affiliation.
    /// </summary>
    public string? ClanAffiliation { get; set; }
}

/// <summary>
/// Data transfer object for contact information.
/// </summary>
public class ContactInformationDto
{
    /// <summary>
    /// The email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// The street address.
    /// </summary>
    public string? Street { get; set; }

    /// <summary>
    /// The city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// The state or province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// The postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// The country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// The phone numbers.
    /// </summary>
    public List<string>? PhoneNumbers { get; set; }
}

/// <summary>
/// Data transfer object for biometric data.
/// </summary>
public class BiometricDataDto
{
    /// <summary>
    /// The type of biometric data.
    /// </summary>
    public BiometricType Type { get; set; }

    /// <summary>
    /// The encrypted biometric template (base64-encoded).
    /// </summary>
    public string? EncryptedTemplate { get; set; }

    /// <summary>
    /// The hash of the biometric data for verification.
    /// </summary>
    public string? Hash { get; set; }

    /// <summary>
    /// The template identifier.
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// The algorithm version used (e.g., "face-3.2.0").
    /// </summary>
    public string? AlgoVersion { get; set; }

    /// <summary>
    /// The template format (e.g., "ISO39794-5:Face").
    /// </summary>
    public string? Format { get; set; }

    /// <summary>
    /// The quality assessment.
    /// </summary>
    public BiometricQuality Quality { get; set; }

    /// <summary>
    /// The signed evidence JWS from Biometric Verification Microservice.
    /// </summary>
    public string? EvidenceJws { get; set; }

    /// <summary>
    /// The PAD (Presentation Attack Detection) score (0..1).
    /// </summary>
    public double? LivenessScore { get; set; }

    /// <summary>
    /// The PAD decision (PASS | FAIL | INCONCLUSIVE).
    /// </summary>
    public string? LivenessDecision { get; set; }

    /// <summary>
    /// The date and time the biometric data was captured.
    /// </summary>
    public DateTime? CapturedAt { get; set; }
}








