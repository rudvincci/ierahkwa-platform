using System.Collections;

namespace Mamey.AmvvaStandards.Tests.DataAttributes;

/// <summary>
/// Provides multiple test cases for ValidateDriverLicenseCard.
/// Implements IEnumerable<object[]> so xUnit can consume it via [ClassData].
/// </summary>
public class DriverLicenseCardTestData : IEnumerable<object[]>
{
    // Returns one test case per yield return.
    public IEnumerator<object[]> GetEnumerator()
    {
        // HAPPY PATH
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABC123", // 6 chars => valid
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "90001"
            },
            true, // expectedValid
            "" // expectedMessageSubstring (none)
        };

        // UNHAPPY PATH: Family name missing
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABC123",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013
            },
            false,
            "Family name is required."
        };
        
        // (2) UNHAPPY PATH: FamilyName too long (exceeds 35)
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = new string('A', 36), // 36 chars
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCD1234",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "90001"
            },
            false,
            "Family name exceeds maximum length of 35."
        };

        // UNHAPPY PATH: LicenseOrIdNumber too short
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "AB", // too short
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013
            },
            false,
            "License number is invalid or out of range (5-16 chars)."
        };
        
        // (3) UNHAPPY PATH: Expiration date <= Issue date
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2025, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1), // same => invalid
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "90001"
            },
            false,
            "Expiration date must be after issue date."
        };
        // (4) UNHAPPY PATH: Future DOB (DOB >= issue date)
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(2030, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "90001"
            },
            false,
            "Date of birth must be before issue date."
        };
        
        // (5) UNHAPPY PATH: Overly long license number (17 chars)
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Smith",
                GivenName = "Jane",
                DateOfBirth = new DateTime(1990, 1, 1),
                IssueDate = new DateTime(2020, 1, 1),
                ExpirationDate = new DateTime(2026, 1, 1),
                LicenseOrIdNumber = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", // 26 chars
                HeightInches = 60,
                DocumentDiscriminator = "DOC999",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "90001"
            },
            false,
            "License number is invalid or out of range (5-16 chars)."
        };
        // UNHAPPY PATH: Height is below 48
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 40, // too short
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013
            },
            false,
            "Height is out of acceptable range (48–96 inches)."
        };

        // UNHAPPY PATH: Jurisdiction rules fail for CA (postal must start w/9, length = 5 or 9)
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = CardDesignRevision.AAMVA2013,
                Jurisdiction = JurisdictionCode.CA,
                PostalCode = "12345" // must start with '9'
            },
            false,
            "Jurisdiction-based validation failed."
        };

        // UNHAPPY PATH: Missing DocumentDiscriminator when Revision is AAMVA2013
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 60,
                Revision = CardDesignRevision.AAMVA2013
            },
            false,
            "Document Discriminator is required for 2013 standard."
        };

        // UNHAPPY PATH: Revision not supported (pretend 999 is a future version)
        yield return new object[]
        {
            new DriverLicenseCard
            {
                FamilyName = "Doe",
                GivenName = "John",
                DateOfBirth = new DateTime(1995, 1, 1),
                IssueDate = new DateTime(2022, 1, 1),
                ExpirationDate = new DateTime(2025, 1, 1),
                LicenseOrIdNumber = "ABCDE123",
                HeightInches = 60,
                DocumentDiscriminator = "DOC123",
                Revision = (CardDesignRevision)999
            },
            false,
            "Unsupported AAMVA revision:"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}