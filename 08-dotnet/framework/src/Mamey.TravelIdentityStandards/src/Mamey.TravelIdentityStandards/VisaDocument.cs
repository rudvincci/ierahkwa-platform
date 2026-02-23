using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards;

/// <summary>
/// Represents a TD2-compliant visa document.
/// </summary>
public class VisaDocument : TravelDocument
{
    public PersonData PersonData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisaDocument"/> class.
    /// </summary>
    public VisaDocument(
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
            (45, 35), // Photo dimensions for TD2 (mm)
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
        PersonData = personData;
        ValidateTD2SpecificRules();
        Mrz = (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, new MrzData(documentNumber, issuingCountry, personData.Nationality, personData.DateOfBirth, expiryDate, personData.Surname,
            personData.GivenNames, personData.Gender, optionalData));
    }
    public VisaMrz Mrz { get; private set; }

    /// <summary>
    /// Validates rules specific to TD2 documents.
    /// </summary>
    private void ValidateTD2SpecificRules()
    {
        if (OptionalData.Length > 15)
            throw new ArgumentException("Optional data must not exceed 15 characters for TD2.");
    }
}
