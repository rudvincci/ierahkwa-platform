using System.Text.RegularExpressions;

namespace Mamey.TravelIdentityStandards;

/// <summary>
/// Base class for all machine-readable travel documents (MRTDs).
/// Compliant with ICAO Doc 9303 standards.
/// </summary>
public abstract class TravelDocument
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TravelDocument"/> class.
    /// </summary>
    /// <param name="documentNumber">Unique document number (1-9 characters).</param>
    /// <param name="issuingCountry">3-character ISO code of the issuing country.</param>
    /// <param name="nationality">3-character ISO code of the holder's nationality.</param>
    /// <param name="dateOfBirth">Date of birth of the document holder.</param>
    /// <param name="expiryDate">Expiration date of the document.</param>
    /// <param name="surname">Primary identifier (surname).</param>
    /// <param name="givenNames">Secondary identifiers (given names).</param>
    /// <param name="optionalData">Optional alphanumeric data (length varies by type).</param>
    /// <param name="gender">Gender of the holder ('M', 'F', or '<').</param>
    /// <param name="photoDimensions">Width and height of the photo in mm.</param>
    /// <param name="photoFormat">Format of the photo (JPEG or PNG).</param>
    /// <param name="photoDpi">DPI of the photo.</param>
    /// <param name="issuerName">Name of the issuing authority.</param>
    /// <param name="issuerCode">3-character ISO code of the issuing authority.</param>
    /// <param name="documentTypeCode">Document type code (e.g., 'P' for passport).</param>
    /// <param name="securityFeatures">Array of security features included.</param>
    /// <param name="issueDate">Date of issuance.</param>
    /// <param name="encoding">Character encoding (default UTF-8).</param>
    /// <param name="isElectronic">Indicates whether the document is electronic (eMRTD).</param>
    /// <param name="biometricData">Biometric data included in the document.</param>
    protected TravelDocument(PersonData personData,
        string documentNumber,
        string issuingCountry,
        DateTime expiryDate,
        string optionalData,
        (int Width, int Height) photoDimensions,
        string photoFormat,
        int photoDpi,
        string issuerName,
        string issuerCode,
        string documentTypeCode,
        string[] securityFeatures,
        DateTime issueDate,
        string encoding = "UTF-8",
        bool isElectronic = false)
    {
        PersonData = personData;
        DocumentNumber = documentNumber;
        IssuingCountry = issuingCountry;
        ExpiryDate = expiryDate;
        OptionalData = optionalData;
        PhotoDimensions = photoDimensions;
        PhotoFormat = photoFormat;
        PhotoDPI = photoDpi;
        IssuerName = issuerName;
        IssuerCode = issuerCode;
        DocumentTypeCode = documentTypeCode;
        SecurityFeatures = securityFeatures ?? throw new ArgumentNullException(nameof(securityFeatures));
        IssueDate = issueDate;
        Encoding = encoding;
        IsElectronic = isElectronic;
        ValidateDocument();
    }

    #region Properties

    public PersonData PersonData { get; }

    /// <summary>Unique document number (1-9 characters).</summary>
    public string DocumentNumber { get; }

    /// <summary>3-character ISO code for the issuing country.</summary>
    public string IssuingCountry { get; }
 

    /// <summary>Expiration date of the document.</summary>
    public DateTime ExpiryDate { get; }
    

    /// <summary>Optional alphanumeric data (length varies by type).</summary>
    public string OptionalData { get; }

    /// <summary>Dimensions of the holder's photo in millimeters.</summary>
    public (int Width, int Height) PhotoDimensions { get; internal set; }

    /// <summary>Format of the holder's photo (JPEG or PNG).</summary>
    public string PhotoFormat { get; internal set; }

    /// <summary>DPI of the holder's photo.</summary>
    public int PhotoDPI { get; internal set; }

    /// <summary>Name of the document issuing authority.</summary>
    public string IssuerName { get; }

    /// <summary>3-character ISO code of the issuing authority.</summary>
    public string IssuerCode { get; }

    /// <summary>Document type code (e.g., 'P' for passport).</summary>
    public string DocumentTypeCode { get; }

    /// <summary>List of security features included in the document.</summary>
    public string[] SecurityFeatures { get; }

    /// <summary>Date when the document was issued.</summary>
    public DateTime IssueDate { get; }

    /// <summary>Character encoding used in the document.</summary>
    public string Encoding { get; }

    /// <summary>Indicates if the document is electronic (eMRTD).</summary>
    public bool IsElectronic { get; }

    #endregion
    
    // Helper Method: Format Date (YYMMDD)
    protected string FormatDate(DateTime date) => date.ToString("yyMMdd");

    #region Validation

    /// <summary>
    /// Validates the document properties based on ICAO Doc 9303.
    /// </summary>
    protected void ValidateDocument()
    {
        ValidateCommonFields();
        ValidatePhoto();
        ValidateIssuerDetails();
        ValidateDocumentTypeSpecificRules();
    }

    private void ValidateCommonFields()
    {
        if (string.IsNullOrWhiteSpace(DocumentNumber) || DocumentNumber.Length > 9)
            throw new ArgumentException("Document number must be between 1 and 9 characters.");

        if (!Regex.IsMatch(IssuingCountry, @"^[A-Z]{3}$"))
            throw new ArgumentException("Issuing country must be a 3-character ISO code.");

        if (!Regex.IsMatch(PersonData.Nationality, @"^[A-Z]{3}$"))
            throw new ArgumentException("Nationality must be a 3-character ISO code.");

        if (PersonData.Gender != "M" && PersonData.Gender != "F" && PersonData.Gender != "<")
            throw new ArgumentException("Gender must be 'M', 'F', or '<' for unspecified.");

        if (ExpiryDate <= PersonData.DateOfBirth)
            throw new ArgumentException("Expiry date must be after the date of birth.");
        if(ExpiryDate < IssueDate)
            throw new ArgumentException("Expiry date must be after the issue date.");
    }

    private void ValidatePhoto()
    {
        if (PhotoDimensions.Width <= 0 || PhotoDimensions.Height <= 0)
            throw new ArgumentException("Photo dimensions must be positive integers.");

        if (string.IsNullOrEmpty(PhotoFormat) || !new[] { "JPEG", "PNG" }.Contains(PhotoFormat.ToUpper()))
            throw new ArgumentException("Photo format must be JPEG or PNG.");

        if (PhotoDPI <= 0)
            throw new ArgumentException("Photo DPI must be a positive integer.");
    }

    private void ValidateIssuerDetails()
    {
        if (string.IsNullOrEmpty(IssuerName))
            throw new ArgumentException("Issuer name cannot be null or empty.");

        if (!Regex.IsMatch(IssuerCode, @"^[A-Z]{3}$"))
            throw new ArgumentException("Issuer code must be a 3-character ISO 3166-1 alpha-3 code.");
    }

    private void ValidateDocumentTypeSpecificRules()
    {
        if (string.IsNullOrEmpty(DocumentTypeCode))
            throw new ArgumentException("Document type code cannot be null or empty.");

        if (IssueDate == DateTime.MinValue)
            throw new ArgumentException("Issue date must be a valid date.");

        if (!new[] { "UTF-8", "ASCII" }.Contains(Encoding.ToUpper()))
            throw new ArgumentException("Encoding must be UTF-8 or ASCII.");
        if (IsElectronic && !PersonData.BiometricData.Any())
        {
            throw new ArgumentException("Biometric data cannot be empty.");
        }
    }

    #endregion
    
    
}

