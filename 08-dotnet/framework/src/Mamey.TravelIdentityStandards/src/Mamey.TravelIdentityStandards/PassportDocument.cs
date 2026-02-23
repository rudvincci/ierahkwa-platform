using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards;

/// <summary>
/// Represents a TD3-compliant passport document.
/// </summary>
public class PassportDocument : TravelDocument
{
    public PersonData PersonData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PassportDocument"/> class.
    /// </summary>
    public PassportDocument(
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
            (35, 45), // Photo dimensions for TD3 (mm)
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
        ValidateTD3SpecificRules();
        Mrz = (PassportMrz) MrzGenerator.GenerateMrz(DocumentType.Passport, new MrzData(documentNumber, issuingCountry, personData.Nationality, personData.DateOfBirth, expiryDate, personData.Surname,
            personData.GivenNames, personData.Gender, optionalData));
    }
    public PassportMrz Mrz { get; private set; }

    /// <summary>
    /// Validates rules specific to TD3 documents.
    /// </summary>
    private void ValidateTD3SpecificRules()
    {
        if (OptionalData.Length > 14)
            throw new ArgumentException("Optional data must not exceed 14 characters for TD3.");
        if (IsElectronic && PersonData.BiometricData == null)
        {
            throw new ArgumentException("Electronic data must have at least one biometric data.");
        }
    }
}
