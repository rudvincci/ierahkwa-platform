namespace Mamey.AmvvaStandards.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // 1) Create a card number generator
        ICardNumberGenerator cardNumberGenerator = new DefaultCardNumberGenerator();

        // 2) Build a new driver license card
        var dlCard = new DriverLicenseCard
        {
            Country = IssuingCountry.USA,
            Jurisdiction = JurisdictionCode.CA,
            FamilyName = "Doe",
            GivenName = "John",
            MiddleNames = "Matthew",
            DateOfBirth = new DateTime(1995, 4, 10),
            IssueDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(5),
            StreetAddress = "123 Main Street",
            StreetAddress2 = "Apt 4B",
            City = "Los Angeles",
            PostalCode = "90001",
            Sex = Sex.Male,
            EyeColor = EyeColor.BLU,
            HairColor = HairColor.BRO,
            HeightInches = 70,
            LicenseClass = "C",
            Restrictions = "NONE",
            Endorsements = "NONE",
            DocumentDiscriminator = "ABC123XYZ",
            Revision = CardDesignRevision.AAMVA2013
        };

        // 3) Assign a generated license number
        var newLicenseNumber = cardNumberGenerator.GenerateCardNumber(
            dlCard.Country,
            dlCard.Jurisdiction,
            dlCard.IsCommercial
        );
        dlCard.AssignCardNumber(newLicenseNumber);

        // 4) Validate the card
        if (!AamvaValidators.ValidateDriverLicenseCard(dlCard, out var dlError))
        {
            Console.WriteLine($"Driver License invalid: {dlError}");
            return;
        }

        // 5) Encode for PDF417
        var dlData = FieldEncodingHelper.EncodeDriverLicenseCard(dlCard);
        var barcodeBytes = BarcodeEncoder.EncodePdf417(dlData);

        Console.WriteLine("Driver License generated and validated successfully!");
        Console.WriteLine($"  License Number: {dlCard.LicenseOrIdNumber}");
        Console.WriteLine($"  Encoded PDF417 Data:\n{dlData}");

        // (Optionally) Write the barcode image to a file:
        // File.WriteAllBytes("DL_Barcode.png", barcodeBytes);


        // ---------------------------------------------------------------
        // Example for an Identification Card
        // ---------------------------------------------------------------
        var idCard = new IdentificationCard
        {
            Country = IssuingCountry.USA,
            Jurisdiction = JurisdictionCode.NV,
            FamilyName = "Smith",
            GivenName = "Jane",
            DateOfBirth = new DateTime(1990, 1, 2),
            IssueDate = DateTime.UtcNow.AddDays(-10),
            ExpirationDate = DateTime.UtcNow.AddYears(10),
            StreetAddress = "456 Oak Ave",
            City = "Las Vegas",
            PostalCode = "89109",
            Sex = Sex.Female,
            EyeColor = EyeColor.HAZ,
            HairColor = HairColor.BLN,
            HeightInches = 65,
            DocumentDiscriminator = "ID999999",
            Revision = CardDesignRevision.AAMVA2010,
            IsRealIdCompliant = true
        };

        // Generate a card number for the ID
        var newIdNumber = cardNumberGenerator.GenerateCardNumber(
            idCard.Country,
            idCard.Jurisdiction,
            isCommercial: false
        );
        idCard.AssignCardNumber(newIdNumber);

        // Validate the ID card
        if (!AamvaValidators.ValidateIdentificationCard(idCard, out var idError))
        {
            Console.WriteLine($"ID Card invalid: {idError}");
            return;
        }

        // Encode for PDF417
        var idData = FieldEncodingHelper.EncodeIdentificationCard(idCard);
        var idBarcodeBytes = BarcodeEncoder.EncodePdf417(idData);

        Console.WriteLine("\nIdentification Card generated and validated successfully!");
        Console.WriteLine($"  ID Number: {idCard.LicenseOrIdNumber}");
        Console.WriteLine($"  Encoded PDF417 Data:\n{idData}");
    }
}