using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Mamey.AmvvaStandards.Tests")]
namespace Mamey.AmvvaStandards;

#region AAMVA Field Definitions

/// <summary>
/// Provides string constants representing AAMVA Data Element Identifiers (DEIs),
/// including those from AAMVA 2020 (and earlier) for DL/ID cards in PDF417 or magstripe formats.
/// 
/// Some DEIs are carried over from older standards (2005, 2009, 2010, 2013).
/// A few are newly introduced or refined in 2020. Always confirm with official documentation.
/// </summary>
public static class AamvaFieldDefinitions
{
    // -------------------------------------------------------------------
    // AAMVA NAME FIELDS
    // -------------------------------------------------------------------

    /// <summary>
    /// Full name (legacy). Some older specs used DAA for the full name combined, 
    /// but it is optional or deprecated in many modern versions.
    /// </summary>
    public const string FullName = "DAA";

    /// <summary>
    /// Family Name (Last Name).
    /// Example: "DOE". Typically up to 35 chars (AAMVA 2020 default limit).
    /// </summary>
    public const string FamilyName = "DCS";

    /// <summary>
    /// Given Name (First Name).
    /// Example: "JOHN". Typically up to 35 chars.
    /// </summary>
    public const string GivenName = "DAC";

    /// <summary>
    /// Middle Name(s).
    /// Example: "MICHAEL". Typically up to 35 chars.
    /// </summary>
    public const string MiddleNames = "DAD";

    /// <summary>
    /// Name Suffix (e.g., JR, SR, I, II, III).
    /// Example: "JR".
    /// </summary>
    public const string NameSuffix = "DCU";

    /// <summary>
    /// Name Prefix (e.g., MR, MRS, MS, DR).
    /// </summary>
    public const string NamePrefix = "DBP";

    // -------------------------------------------------------------------
    // ADDRESS FIELDS (MAILING / RESIDENCE)
    // -------------------------------------------------------------------

    /// <summary>
    /// Mailing Street Address 1.
    /// In AAMVA 2020, still "DAG". Commonly required.
    /// </summary>
    public const string StreetAddress1 = "DAG";

    /// <summary>
    /// Mailing Street Address 2 (apartment, suite, unit).
    /// </summary>
    public const string StreetAddress2 = "DAH";

    /// <summary>
    /// Mailing City.
    /// </summary>
    public const string City = "DAI";

    /// <summary>
    /// Mailing Jurisdiction Code (State/Province).
    /// e.g., "CA" or "BC".
    /// </summary>
    public const string JurisdictionCode = "DAJ";

    /// <summary>
    /// Mailing Postal Code (ZIP/Postal).
    /// Often up to 11 chars, e.g., "12345-6789".
    /// </summary>
    public const string PostalCode = "DAK";

    /// <summary>
    /// Country Identification.
    /// For U.S., typically "USA".
    /// </summary>
    public const string Country = "DCG";

    /// <summary>
    /// Residence Street Address 1.
    /// Not all states differentiate mailing vs. residence in the PDF417 data.
    /// </summary>
    public const string ResidenceStreetAddress1 = "DAL";

    /// <summary>
    /// Residence Street Address 2.
    /// </summary>
    public const string ResidenceStreetAddress2 = "DAM";

    /// <summary>
    /// Residence City.
    /// </summary>
    public const string ResidenceCity = "DAN";

    /// <summary>
    /// Residence Jurisdiction Code (State/Province).
    /// </summary>
    public const string ResidenceJurisdictionCode = "DAO";

    /// <summary>
    /// Residence Postal Code (ZIP/Postal).
    /// </summary>
    public const string ResidencePostalCode = "DAP";

    // -------------------------------------------------------------------
    // DATES
    // -------------------------------------------------------------------

    /// <summary>
    /// License/ID Expiration Date (YYYYMMDD).
    /// </summary>
    public const string ExpirationDate = "DBA";

    /// <summary>
    /// Date of Birth (YYYYMMDD).
    /// </summary>
    public const string DateOfBirth = "DBB";

    /// <summary>
    /// License/ID Issue Date (YYYYMMDD).
    /// </summary>
    public const string IssueDate = "DBD";

    /// <summary>
    /// Date the name was changed or updated (sometimes used for the last name update).
    /// Field usage can vary. In older specs, "DBE" or "DBF" might appear differently.
    /// </summary>
    public const string LastNameUpdate = "DBE";

    // -------------------------------------------------------------------
    // CORE LICENSE/ID INFORMATION
    // -------------------------------------------------------------------

    /// <summary>
    /// Driver’s License / ID Number.
    /// </summary>
    public const string LicenseNumber = "DAQ";

    /// <summary>
    /// License Classification Code (e.g., A, B, C, D).
    /// In many specs: "DAR".
    /// </summary>
    public const string LicenseClass = "DAR";

    /// <summary>
    /// License Restrictions Code (AAMVA standard set or jurisdiction-specific).
    /// e.g., "CORRECTIVE LENSES".
    /// </summary>
    public const string Restrictions = "DAS";

    /// <summary>
    /// License Endorsements Code.
    /// e.g., "T" (Double/Triple Trailers), "P" (Passengers), etc.
    /// </summary>
    public const string Endorsements = "DAT";

    /// <summary>
    /// Document Discriminator (DD).
    /// Uniquely identifies a particular document issued to the customer.
    /// </summary>
    public const string DocumentDiscriminator = "DCF";

    /// <summary>
    /// Audit Information.
    /// An internal number for the issuing transaction or audit trail.
    /// </summary>
    public const string AuditInformation = "DCJ";

    /// <summary>
    /// Inventory Control Number.
    /// Internal tracking number for the physical card stock.
    /// </summary>
    public const string InventoryControlNumber = "DCK";

    /// <summary>
    /// Place of Birth (City/State/Country).
    /// Often "DCI" in older or some 2020 references; usage can vary.
    /// </summary>
    public const string PlaceOfBirth = "DCI";

    // -------------------------------------------------------------------
    // PHYSICAL DESCRIPTORS
    // -------------------------------------------------------------------

    /// <summary>
    /// Gender / Sex.
    /// AAMVA: "1" = Male, "2" = Female, "9" = Not Specified/Non-Binary in some 2020 references.
    /// </summary>
    public const string Sex = "DBC";

    /// <summary>
    /// Height (inches or centimeters, depending on jurisdiction).
    /// Typically stored as an integer (e.g., 70 for 5'10").
    /// </summary>
    public const string Height = "DAU";

    /// <summary>
    /// Eye Color.
    /// Values: BLK, BLU, BRO, GRN, GRY, HAZ, MAR, PNK, DIC (Dichromatic), UNK.
    /// </summary>
    public const string EyeColor = "DAY";

    /// <summary>
    /// Hair Color.
    /// Values: BAL, BLK, BLN, BRO, GRY, RED, SDY, WHI, UNK.
    /// </summary>
    public const string HairColor = "DAZ";

    /// <summary>
    /// Weight in pounds (optional in some states).
    /// </summary>
    public const string WeightPounds = "DAV";

    /// <summary>
    /// Weight in kilograms (optional in some states).
    /// </summary>
    public const string WeightKilograms = "DAW";

    // -------------------------------------------------------------------
    // NAME LEGACY/ALTERNATE FIELDS
    // -------------------------------------------------------------------

    /// <summary>
    /// Driver Last Name (Legacy field).
    /// In some older specs, DAB was specifically last name.
    /// </summary>
    public const string LegacyLastName = "DAB";

    /// <summary>
    /// Driver First Name (Legacy).
    /// </summary>
    public const string LegacyFirstName = "DAC";

    /// <summary>
    /// Driver Middle Name (Legacy).
    /// </summary>
    public const string LegacyMiddleName = "DAD";

    // -------------------------------------------------------------------
    // REAL ID & 2020-ERA FIELDS
    // -------------------------------------------------------------------

    /// <summary>
    /// Under 18 Until (YYYYMMDD).
    /// AAMVA 2020 references "DDH" for Under 18 cutoffs in some versions.
    /// </summary>
    public const string Under18Until = "DDH";

    /// <summary>
    /// Under 21 Until (YYYYMMDD).
    /// AAMVA 2020 references "DDI" for Under 21 cutoffs.
    /// </summary>
    public const string Under21Until = "DDI";

    /// <summary>
    /// Organ Donor Indicator (some states).
    /// AAMVA 2020 references “DDK” in some contexts, older references “DBH”.
    /// </summary>
    public const string OrganDonor = "DBH";

    /// <summary>
    /// Veteran Indicator (some states).
    /// AAMVA 2020 references “DDL” or “DCO” in some contexts.
    /// </summary>
    public const string VeteranIndicator = "DCO"; // Or "DDL" in certain jurisdictions

    /// <summary>
    /// Federal Non-Compliance Indicator (for non-REAL ID).
    /// In 2020, might appear as “DDE” or “DDC”. Usage is jurisdictional.
    /// </summary>
    public const string NonRealIdIndicator = "DDE";

    /// <summary>
    /// Limited Term Indicator (often for temporary visas).
    /// AAMVA 2020 references “DDF” for limited-term credentials.
    /// </summary>
    public const string LimitedTermIndicator = "DDF";

    /// <summary>
    /// Compliance Type.
    /// AAMVA 2020 references “DDD” or “DDG” for indicating REAL ID compliance types 
    /// (REAL ID, Non-REAL, Enhanced, etc.).
    /// </summary>
    public const string ComplianceType = "DDD";

    // -------------------------------------------------------------------
    // AAMVA VERSION / JURISDICTION-SPECIFIC
    // -------------------------------------------------------------------

    /// <summary>
    /// AAMVA Version Number (2-digit) indicating which standard is used in the PDF417 data.
    /// e.g., "08" for 2009, "10" for 2010, "13" for 2013, "20" for 2020, etc.
    /// </summary>
    public const string AamvaVersionNumber = "DCA";

    /// <summary>
    /// Jurisdiction-specific vehicle class.
    /// Some states store local class definitions in “DCB”.
    /// </summary>
    public const string JurisdictionVehicleClass = "DCB";

    /// <summary>
    /// Jurisdiction-specific endorsements code.
    /// </summary>
    public const string JurisdictionEndorsements = "DCD";

    /// <summary>
    /// Jurisdiction-specific restriction codes.
    /// </summary>
    public const string JurisdictionRestrictions = "DCE";

    // -------------------------------------------------------------------
    // OPTIONAL / OTHER FIELDS
    // -------------------------------------------------------------------

    /// <summary>
    /// Customer Identifier (some states use this in place of the official 
    /// license number or as an internal ID).
    /// Field "DCI" or "DCF" in older references. Potentially duplicative – must confirm version usage.
    /// </summary>
    public const string CustomerId = "DCI";

    /// <summary>
    /// Alias / AKA Family Name.
    /// </summary>
    public const string AliasFamilyName = "DBN";

    /// <summary>
    /// Alias / AKA Given Name.
    /// </summary>
    public const string AliasGivenName = "DBO";

    /// <summary>
    /// Alias / AKA Suffix.
    /// </summary>
    public const string AliasSuffix = "DBS";
}

#endregion

#region Enums & Specifications

/// <summary>
/// Enumerates known AAMVA card design revision levels.
/// Adjust as needed based on your jurisdiction or version(s).
/// </summary>
public enum CardDesignRevision
{
    AAMVA2005 = 1,
    AAMVA2009,
    AAMVA2010,
    AAMVA2012,
    AAMVA2013,
    // Add more if needed (AAMVA2016, 2020, etc.)
}

/// <summary>
/// Stores specification-level constants and rules for each known revision.
/// For example, PDF417 header strings, mandatory fields, and other versioned details.
/// </summary>
public static class AamvaSpecifications
{
    public static class Pdf417Headers
    {
        // Commonly, the header includes an '@', record separators, 
        // or a special version ID. These are examples only:
        public const string AAMVA2005Header = "@\n\x1e\nANSI 636000050002DL00410278ZA03200004DLDAQ";
        public const string AAMVA2009Header = "@\n\x1e\nANSI 636000090002DL00410278ZA03200004DLDAQ";

        public const string AAMVA2010Header = "@\n\x1e\nANSI 636000100002DL00410278ZA03200004DLDAQ";
        // ...
    }

    /// <summary>
    /// Returns the recommended PDF417 header for a specified design revision.
    /// </summary>
    public static string GetPdf417Header(CardDesignRevision revision)
    {
        return revision switch
        {
            CardDesignRevision.AAMVA2005 => Pdf417Headers.AAMVA2005Header,
            CardDesignRevision.AAMVA2009 => Pdf417Headers.AAMVA2009Header,
            CardDesignRevision.AAMVA2010 => Pdf417Headers.AAMVA2010Header,
            // ...
            _ => Pdf417Headers.AAMVA2010Header // default fallback
        };
    }
}

/// <summary>
/// ISO-3166 country codes or your internal representation.
/// </summary>
public enum IssuingCountry
{
    USA,
    CAN,
    MEX,
    // ...
}

/// <summary>
/// U.S. States/Territories (for demonstration).
/// </summary>
public enum JurisdictionCode
{
    AL,
    AK,
    AZ,
    AR,
    CA,
    CO,
    CT,
    DE,
    FL,
    GA,
    HI,
    ID,
    IL,
    IN,
    IA,
    KS,
    KY,
    LA,
    ME,
    MD,
    MA,
    MI,
    MN,
    MS,
    MO,
    MT,
    NE,
    NV,
    NH,
    NJ,
    NM,
    NY,
    NC,
    ND,
    OH,
    OK,
    OR,
    PA,
    RI,
    SC,
    SD,
    TN,
    TX,
    UT,
    VT,
    VA,
    WA,
    WV,
    WI,
    WY,
    DC,
    GU,
    PR,
    VI,
    AS,
    MP // territories, etc.
}

/// <summary>
/// Additional enumerations for physical descriptions, if needed.
/// Examples from AAMVA.
/// </summary>
public enum EyeColor
{
    BLK,
    BLU,
    BRO,
    GRN,
    GRY,
    HAZ,
    MAR,
    PNK,
    DIC, // Dichromatic
    UNK
}

public enum HairColor
{
    BAL,
    BLK,
    BLN,
    BRO,
    GRY,
    RED,
    SDY,
    WHI,
    UNK
}

public enum Sex
{
    Male = 1,
    Female = 2,
    NotSpecified = 9
}

#endregion

#region Card Number Generation

/// <summary>
/// Defines a strategy for generating AAMVA-compliant card numbers.
/// </summary>
public interface ICardNumberGenerator
{
    /// <summary>
    /// Generates a new license/ID number per AAMVA or jurisdiction rules.
    /// </summary>
    /// <param name="country">Issuing country.</param>
    /// <param name="jurisdiction">Jurisdiction (state/province).</param>
    /// <param name="isCommercial">Whether this is for a commercial driver’s license.</param>
    /// <returns>A unique alphanumeric license/ID string.</returns>
    string GenerateCardNumber(IssuingCountry country, JurisdictionCode jurisdiction, bool isCommercial = false);
}

/// <summary>
/// A baseline card number generator demonstrating how you might
/// structure a predictable yet unique card numbering scheme for production.
/// 
/// Uses a cryptographically secure random generator to avoid predictable sequences.
/// In production, you should store assigned numbers in a DB to guarantee uniqueness.
/// </summary>
public class DefaultCardNumberGenerator : ICardNumberGenerator
{
    // For real production, store previously used IDs in a DB or distributed cache 
    // to avoid collisions or re-issuance.

    public string GenerateCardNumber(IssuingCountry country, JurisdictionCode jurisdiction, bool isCommercial = false)
    {
        // Start with the state code
        var statePrefix = jurisdiction.ToString().ToUpperInvariant();

        // Example: For commercial, add a "C" prefix or suffix
        var commercialIndicator = isCommercial ? "C" : "D";

        // Some pattern for random part. We'll use a cryptographically secure RNG 
        // for better uniqueness and unpredictability.
        var randomPart = GetRandomNumericString(6); // e.g., 6-digit random
        var yearPart = DateTime.UtcNow.Year.ToString();

        var partial = $"{statePrefix}{commercialIndicator}{yearPart}{randomPart}";

        // Compute check digit for final
        var checkDigit = ComputeCheckDigit(partial);
        var final = partial + checkDigit;

        // In real production, you would:
        // 1) Check if final is already used (DB lookup).
        // 2) If it is, generate again or handle collision.
        // For demonstration, we just return final.
        return final;
    }

    private static string GetRandomNumericString(int length)
    {
        // Generate random numeric string of given length using a secure RNG
        byte[] randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var sb = new StringBuilder(length);
        foreach (var b in randomBytes)
        {
            // Convert each byte to 0-9
            sb.Append(b % 10);
        }

        return sb.ToString();
    }

    private int ComputeCheckDigit(string input)
    {
        // Simple example mod 10 check digit.
        // In real scenarios, some states have more complex algorithms.
        int sum = 0;
        foreach (char c in input)
        {
            // Convert char c into numeric value or ASCII code.
            sum += c;
        }

        return sum % 10;
    }
}

#endregion

#region Models

/// <summary>
/// Base class capturing common AAMVA fields for driver’s licenses or ID cards.
/// </summary>
public abstract class BaseAamvaCard
{
    // Mandatory Core Fields
    public string LicenseOrIdNumber { get; internal protected set; } = string.Empty;
    public IssuingCountry Country { get; set; }
    public JurisdictionCode Jurisdiction { get; set; }

    // Name Fields
    public string FamilyName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string MiddleNames { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty; // e.g. "Mr.", "Ms."
    public string Suffix { get; set; } = string.Empty; // e.g. "Jr.", "Sr.", "III"

    // Mailing Address
    public string StreetAddress { get; set; } = string.Empty;
    public string StreetAddress2 { get; set; } = string.Empty; // Apt/Unit
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    // Dates
    public DateTime DateOfBirth { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Physical Descriptors
    public Sex Sex { get; set; } = Sex.NotSpecified;
    public EyeColor EyeColor { get; set; } = EyeColor.UNK;
    public HairColor HairColor { get; set; } = HairColor.UNK;
    public int HeightInches { get; set; } // or store height in centimeters

    // AAMVA Version
    public CardDesignRevision Revision { get; set; } = CardDesignRevision.AAMVA2013;

    // Additional placeholders 
    public string DocumentDiscriminator { get; set; } = string.Empty;
    public string AuditInformation { get; set; } = string.Empty;
    public string InventoryControlNumber { get; set; } = string.Empty;

    // A simple method to assign the ID number after generation
    public void AssignCardNumber(string cardNumber)
    {
        LicenseOrIdNumber = cardNumber;
    }
}

/// <summary>
/// Represents a standard Driver License under AAMVA standards.
/// </summary>
public class DriverLicenseCard : BaseAamvaCard
{
    public string LicenseClass { get; set; } = string.Empty;
    public string Endorsements { get; set; } = string.Empty;
    public string Restrictions { get; set; } = string.Empty;

    public bool IsCommercial { get; set; } = false;
}

/// <summary>
/// Represents a U.S. Identification Card (non-driver) under AAMVA standards.
/// </summary>
public class IdentificationCard : BaseAamvaCard
{
    // Additional ID-specific fields if needed, e.g., RealID indicators, etc.
    public bool IsRealIdCompliant { get; set; }
}

/// <summary>
/// Represents a U.S. Commercial Driver’s License (CDL).
/// </summary>
public class CommercialDriverLicense : DriverLicenseCard
{
    public CommercialDriverLicense()
    {
        IsCommercial = true;
    }

    // Additional fields: e.g., endorsements specific to commercial vehicles (Hazmat, Tank, etc.)
    public bool HazardousMaterialsEndorsement { get; set; }

    public bool TankVehicleEndorsement { get; set; }
    // ...
}

#endregion

#region Validation

/// <summary>
/// Contains validation routines to ensure compliance with AAMVA fields and rules.
/// </summary>
public static class AamvaValidators
{
    /// <summary>
    /// Validate a driver license card using generic AAMVA rules plus possible 
    /// state-level rules or version-specific checks.
    /// </summary>
    public static bool ValidateDriverLicenseCard(DriverLicenseCard card, out string errorMessage)
    {
        if (!CheckVersionCompliance(card, out errorMessage))
        {
            return false;
        }

        // 1) General field presence checks
        if (string.IsNullOrWhiteSpace(card.FamilyName))
        {
            errorMessage = "Family name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(card.GivenName))
        {
            errorMessage = "Given name is required.";
            return false;
        }

        if (card.ExpirationDate <= card.IssueDate)
        {
            errorMessage = "Expiration date must be after issue date.";
            return false;
        }

        if (card.DateOfBirth >= card.IssueDate)
        {
            errorMessage = "Date of birth must be before issue date.";
            return false;
        }

        // 2) Check length constraints (typical AAMVA might allow up to 35 chars for names)
        if (card.FamilyName.Length > 35)
        {
            errorMessage = "Family name exceeds maximum length of 35.";
            return false;
        }

        // 3) Validate License Number format (some states have alpha-numeric constraints)
        if (!Regex.IsMatch(card.LicenseOrIdNumber, "^[A-Za-z0-9]{5,16}$"))
        {
            errorMessage = "License number is invalid or out of range (5-16 chars).";
            return false;
        }

        // 4) Possibly check mandatory fields by revision. Example for AAMVA 2013:
        if (card.Revision == CardDesignRevision.AAMVA2013)
        {
            // Ensure DocumentDiscriminator is present
            if (string.IsNullOrWhiteSpace(card.DocumentDiscriminator))
            {
                errorMessage = "Document Discriminator is required for 2013 standard.";
                return false;
            }
        }

        // 5) Physical descriptor checks, e.g., height must be between 48in and 96in
        if (card.HeightInches < 48 || card.HeightInches > 96)
        {
            errorMessage = "Height is out of acceptable range (48–96 inches).";
            return false;
        }

        // 6) Additional rules. For example, some states do not allow certain 
        // license classes for under 18, etc. 
        if (card.IsCommercial && (DateTime.UtcNow.Year - card.DateOfBirth.Year) < 21)
        {
            errorMessage = "Commercial licenses require age 21+.";
            return false;
        }

        // 7) Optional: Jurisdiction-based rules
        if (!JurisdictionRules.Validate(card))
        {
            errorMessage = "Jurisdiction-based validation failed.";
            return false;
        }

        // If we reach here, everything passes
        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Validate an identification card using generic AAMVA rules plus possible 
    /// version-specific checks.
    /// </summary>
    public static bool ValidateIdentificationCard(IdentificationCard card, out string errorMessage)
    {
        if (!CheckVersionCompliance(card, out errorMessage))
        {
            return false;
        }

        // Similar checks, except no license class or endorsements
        if (string.IsNullOrWhiteSpace(card.FamilyName))
        {
            errorMessage = "Family name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(card.GivenName))
        {
            errorMessage = "Given name is required.";
            return false;
        }

        if (card.ExpirationDate <= card.IssueDate)
        {
            errorMessage = "Expiration date must be after issue date.";
            return false;
        }

        if (card.DateOfBirth >= card.IssueDate)
        {
            errorMessage = "Date of birth must be before issue date.";
            return false;
        }

        // Name length
        if (card.FamilyName.Length > 35)
        {
            errorMessage = "Family name exceeds maximum length of 35.";
            return false;
        }

        // ID Number format
        if (!Regex.IsMatch(card.LicenseOrIdNumber, "^[A-Za-z0-9]{5,16}$"))
        {
            errorMessage = "ID number is invalid or out of range (5-16 chars).";
            return false;
        }

        // Physical descriptors optional but if provided, check ranges
        if (card.HeightInches != 0 && (card.HeightInches < 48 || card.HeightInches > 96))
        {
            errorMessage = "Height is out of acceptable range (48–96 inches).";
            return false;
        }

        // Check version-specific fields
        if (card.Revision >= CardDesignRevision.AAMVA2010
            && string.IsNullOrWhiteSpace(card.DocumentDiscriminator))
        {
            errorMessage = "Document Discriminator is required for AAMVA 2010+.";
            return false;
        }

        // Jurisdiction-based checks
        if (!JurisdictionRules.Validate(card))
        {
            errorMessage = "Jurisdiction-based validation failed.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Simple example to ensure the card’s Revision is recognized and 
    /// hasn't been incorrectly set. Extend for real AAMVA revision rules.
    /// </summary>
    private static bool CheckVersionCompliance(BaseAamvaCard card, out string errorMessage)
    {
        // Example: if a future revision is set but your code only supports up to 2013:
        if ((int)card.Revision > (int)CardDesignRevision.AAMVA2013)
        {
            errorMessage = $"Unsupported AAMVA revision: {card.Revision}. Update your code or specs.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}

/// <summary>
/// Illustrates how certain states may have additional rules.
/// This is purely an example with a simple postal-code constraint for California.
/// In real usage, store these rules in config or a DB for easier updates.
/// </summary>
public static class JurisdictionRules
{
    public static bool Validate(BaseAamvaCard card)
    {
        // For demonstration:
        // If jurisdiction is CA, the postal code must start with 9
        // or have length 5 or 9. You could refine this further.
        if (card.Jurisdiction == JurisdictionCode.CA)
        {
            if (!card.PostalCode.StartsWith("9"))
            {
                return false;
            }

            // Example: ensure length is 5 or 9
            if (card.PostalCode.Length != 5 && card.PostalCode.Length != 9 && !card.PostalCode.Contains("-"))
            {
                return false;
            }
        }
        // Similarly for other states:
        // if (card.Jurisdiction == JurisdictionCode.NY) {...}

        return true;
    }
}

#endregion

#region Field Encoding (PDF417)

/// <summary>
/// Contains routines to convert our in-memory card objects into 
/// AAMVA-standard PDF417 text blocks.
/// </summary>
public static class FieldEncodingHelper
{
    // For real-world usage, AAMVA typically uses ASCII 0x1E as a segment terminator
    // (record separator), and 0x1C or 0x1D as subfile delimiters, etc.
    // This is a simplified demonstration. Adjust for your version's exact structure.
    private const char SEGMENT_TERMINATOR = (char)0x1E;
    private const char DATA_ELEMENT_SEPARATOR = (char)0x1D;

    /// <summary>
    /// Encodes a driver license into a string following AAMVA PDF417 guidelines.
    /// This example is simplified. In practice, you must carefully follow the subfile structure,
    /// version numbers, and mandatory fields for each revision.
    /// </summary>
    public static string EncodeDriverLicenseCard(DriverLicenseCard card)
    {
        var sb = new StringBuilder();

        // 1) Start with the recommended header for the revision
        sb.Append(AamvaSpecifications.GetPdf417Header(card.Revision));

        // 2) Insert the Mandatory Subfile Designation
        // Typically "DL" for Driver License or "ID" for Identification card.
        sb.Append("DL");
        sb.Append(DATA_ELEMENT_SEPARATOR);

        // 3) Optionally encode the version number
        sb.Append(AamvaFieldDefinitions.AamvaVersionNumber)
            .Append(GetVersionNumber(card.Revision))
            .Append(SEGMENT_TERMINATOR);

        // 4) Now append fields using their AAMVA codes.
        sb.Append(AamvaFieldDefinitions.LicenseNumber)
            .Append(card.LicenseOrIdNumber)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.FamilyName)
            .Append(card.FamilyName)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.GivenName)
            .Append(card.GivenName)
            .Append(SEGMENT_TERMINATOR);

        if (!string.IsNullOrWhiteSpace(card.MiddleNames))
        {
            sb.Append(AamvaFieldDefinitions.MiddleNames)
                .Append(card.MiddleNames)
                .Append(SEGMENT_TERMINATOR);
        }

        if (!string.IsNullOrWhiteSpace(card.Suffix))
        {
            sb.Append(AamvaFieldDefinitions.NameSuffix)
                .Append(card.Suffix)
                .Append(SEGMENT_TERMINATOR);
        }

        // Address fields
        sb.Append(AamvaFieldDefinitions.StreetAddress1)
            .Append(card.StreetAddress)
            .Append(SEGMENT_TERMINATOR);

        if (!string.IsNullOrWhiteSpace(card.StreetAddress2))
        {
            sb.Append(AamvaFieldDefinitions.StreetAddress2)
                .Append(card.StreetAddress2)
                .Append(SEGMENT_TERMINATOR);
        }

        sb.Append(AamvaFieldDefinitions.City)
            .Append(card.City)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.JurisdictionCode)
            .Append(card.Jurisdiction)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.PostalCode)
            .Append(card.PostalCode)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Country)
            .Append(card.Country)
            .Append(SEGMENT_TERMINATOR);

        // Dates - Format typically YYYYMMDD
        sb.Append(AamvaFieldDefinitions.DateOfBirth)
            .Append(card.DateOfBirth.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.IssueDate)
            .Append(card.IssueDate.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.ExpirationDate)
            .Append(card.ExpirationDate.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        // Physical descriptors
        sb.Append(AamvaFieldDefinitions.Sex)
            .Append(((int)card.Sex).ToString())
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.EyeColor)
            .Append(card.EyeColor)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.HairColor)
            .Append(card.HairColor)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Height)
            .Append(card.HeightInches.ToString())
            .Append(SEGMENT_TERMINATOR);

        // Driver license specifics
        sb.Append(AamvaFieldDefinitions.LicenseClass)
            .Append(card.LicenseClass)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Restrictions)
            .Append(card.Restrictions)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Endorsements)
            .Append(card.Endorsements)
            .Append(SEGMENT_TERMINATOR);

        // Document Discriminator, Audit Info, etc.
        sb.Append(AamvaFieldDefinitions.DocumentDiscriminator)
            .Append(card.DocumentDiscriminator)
            .Append(SEGMENT_TERMINATOR);

        // ...Add any additional fields your revision mandates

        return sb.ToString();
    }

    /// <summary>
    /// Encodes an identification card into a string following AAMVA PDF417 guidelines.
    /// This example omits license-specific fields.
    /// </summary>
    public static string EncodeIdentificationCard(IdentificationCard card)
    {
        var sb = new StringBuilder();
        sb.Append(AamvaSpecifications.GetPdf417Header(card.Revision));

        // Subfile designator: "ID" for identification card
        sb.Append("ID");
        sb.Append(DATA_ELEMENT_SEPARATOR);

        sb.Append(AamvaFieldDefinitions.AamvaVersionNumber)
            .Append(GetVersionNumber(card.Revision))
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.LicenseNumber)
            .Append(card.LicenseOrIdNumber)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.FamilyName)
            .Append(card.FamilyName)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.GivenName)
            .Append(card.GivenName)
            .Append(SEGMENT_TERMINATOR);

        if (!string.IsNullOrWhiteSpace(card.MiddleNames))
        {
            sb.Append(AamvaFieldDefinitions.MiddleNames)
                .Append(card.MiddleNames)
                .Append(SEGMENT_TERMINATOR);
        }

        if (!string.IsNullOrWhiteSpace(card.Suffix))
        {
            sb.Append(AamvaFieldDefinitions.NameSuffix)
                .Append(card.Suffix)
                .Append(SEGMENT_TERMINATOR);
        }

        // Address fields
        sb.Append(AamvaFieldDefinitions.StreetAddress1)
            .Append(card.StreetAddress)
            .Append(SEGMENT_TERMINATOR);

        if (!string.IsNullOrWhiteSpace(card.StreetAddress2))
        {
            sb.Append(AamvaFieldDefinitions.StreetAddress2)
                .Append(card.StreetAddress2)
                .Append(SEGMENT_TERMINATOR);
        }

        sb.Append(AamvaFieldDefinitions.City)
            .Append(card.City)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.JurisdictionCode)
            .Append(card.Jurisdiction)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.PostalCode)
            .Append(card.PostalCode)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Country)
            .Append(card.Country)
            .Append(SEGMENT_TERMINATOR);

        // Dates
        sb.Append(AamvaFieldDefinitions.DateOfBirth)
            .Append(card.DateOfBirth.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.IssueDate)
            .Append(card.IssueDate.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.ExpirationDate)
            .Append(card.ExpirationDate.ToString("yyyyMMdd"))
            .Append(SEGMENT_TERMINATOR);

        // Physical descriptors
        sb.Append(AamvaFieldDefinitions.Sex)
            .Append(((int)card.Sex).ToString())
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.EyeColor)
            .Append(card.EyeColor)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.HairColor)
            .Append(card.HairColor)
            .Append(SEGMENT_TERMINATOR);

        sb.Append(AamvaFieldDefinitions.Height)
            .Append(card.HeightInches.ToString())
            .Append(SEGMENT_TERMINATOR);

        // Document Discriminator
        sb.Append(AamvaFieldDefinitions.DocumentDiscriminator)
            .Append(card.DocumentDiscriminator)
            .Append(SEGMENT_TERMINATOR);

        // ...Add any additional fields your revision mandates

        return sb.ToString();
    }

    // Example: You might map enum to actual version digits for PDF417:
    private static string GetVersionNumber(CardDesignRevision revision)
    {
        return revision switch
        {
            CardDesignRevision.AAMVA2005 => "05",
            CardDesignRevision.AAMVA2009 => "09",
            CardDesignRevision.AAMVA2010 => "10",
            CardDesignRevision.AAMVA2012 => "12",
            CardDesignRevision.AAMVA2013 => "13",
            _ => "10" // fallback or your latest default
        };
    }
}

/// <summary>
/// Encodes a string into PDF417 barcode. Return the raw image bytes (e.g. PNG) or a memory stream.
/// In practice, you'd integrate with a robust 3rd-party library or official AAMVA solution.
/// </summary>
public static class BarcodeEncoder
{
    public static byte[] EncodePdf417(string data)
    {
        // Pseudocode with a hypothetical library:
        // var generator = new SomePdf417GeneratorLibrary();
        // generator.SetData(data);
        // generator.SetOptions( /* e.g. error correction, row/column configuration */ );
        // return generator.GenerateImageAsBytes();

        return Array.Empty<byte>(); // Placeholder
    }
}

#endregion