using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards;

/// <summary>
/// ICAO TD1
/// </summary>
/// <summary>
/// Represents a TD1-compliant identity card document.
/// </summary>
public class IdCardTravelDocument : TravelDocument
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdCardTravelDocument"/> class.
    /// </summary>
    public IdCardTravelDocument(
        PersonData personData,
        string documentNumber,
        string issuingCountry,
        DateTime expiryDate,
        string optionalData,
        string photoFormat,
        string issuerName,
        string issuerCode,
        string documentTypeCode,
        string[] securityFeatures,
        DateTime issueDate,
        string encoding = "UTF-8",
        bool isElectronic = false)
        : base(
            personData,
            documentNumber,
            issuingCountry,
            expiryDate,
            optionalData,
            (35, 45), // Photo dimensions for TD1 (mm)
            photoFormat,
            300, // Standard DPI
            issuerName,
            issuerCode,
            documentTypeCode,
            securityFeatures,
            issueDate,
            encoding,
            isElectronic)
    {
        ValidateTD1SpecificRules();
        Mrz = (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, new MrzData(documentNumber, issuingCountry, personData.Nationality, personData.DateOfBirth, expiryDate, personData.Surname,
            personData.GivenNames, personData.Gender, optionalData));
    }
    public IdCardMrz? Mrz { get; private set; }

    /// <summary>
    /// Validates rules specific to TD1 documents.
    /// </summary>
    private void ValidateTD1SpecificRules()
    {
        if (OptionalData.Length > 15)
            throw new ArgumentException("Optional data must not exceed 15 characters for TD1.");
    }
}
