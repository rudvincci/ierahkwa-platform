using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards.Tests;

public class PassportMrzGeneratorTests
{
    [Fact]
    public void TD3MRZGenerator_ValidData_ReturnsCorrectFormat()
    {
        var data = new MrzData("123456789", "GBR", "GBR",
            new DateTime(1990, 1, 1), new DateTime(2030, 1, 1), "DOE",
            "JOHN", "M", "1234567890");

        var mrz = (PassportMrz) MrzGenerator.GenerateMrz(DocumentType.Passport, data);

        Assert.Equal(typeof(PassportMrz), mrz.GetType());

        // Line 1: Document type, issuing country, surname, given names
        Assert.Matches(@"^P<GBRDOE<<JOHN<<<<<<<<<<<<<<<<<<<<<<<<<<$", mrz.Line1);

        // Line 2: Document number, nationality, DOB, gender, expiry date, optional data, composite checksum
        Assert.Matches(@"^123456789[0-9]GBR[0-9]{6}[0-9][MF][0-9]{6}[0-9][0-9]{10}[0-9]$", mrz.Line2);
    }

    [Fact]
    public void TD3MRZGenerator_CompositeChecksum_ValidatesCorrectly()
    {
        var data = new MrzData("987654321", "USA", "USA",
            new DateTime(1980, 7, 20), new DateTime(2025, 7, 20), "SMITH",
            "JANE", "F", "OPTIONAL");

        var mrz = (PassportMrz) MrzGenerator.GenerateMrz(DocumentType.Passport, data);
 
        Assert.NotNull(mrz);

        // Remove padding before calculating checksum
        var compositeString = mrz.Line2.TrimEnd('<');
        var compositeStringWithoutChecksum = compositeString.Substring(0, mrz.Line2.TrimEnd('<').Length-1);
        var checkDigit = MRZUtils.CalculateCheckDigit(compositeStringWithoutChecksum);
        
        // Assert the last character matches the computed checksum
        Assert.EndsWith(checkDigit.ToString(), compositeString);
    }

    [Fact]
    public void TD3MRZGenerator_InvalidNationality_ThrowsException()
    {
        var data = new MrzData("123456789", "USA", "INVALID",
            new DateTime(1980, 7, 20), new DateTime(2025, 7, 20), "SMITH",
            "JANE", "F", "123456");
        

        Assert.Throws<ArgumentException>(() => (PassportMrz) MrzGenerator.GenerateMrz(DocumentType.Passport, data));
    }
    [Fact]
    public void TD3MRZGenerator_InvalidOptionalDataLength_ThrowsException()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1980, 7, 20), new DateTime(2025, 7, 20), "SMITH",
            "JANE", "F", "TOOLONGOPTIONALDATA123");
        
        Assert.Throws<ArgumentException>(() => (PassportMrz) MrzGenerator.GenerateMrz(DocumentType.Passport, data));
    }
    [Theory]
    [InlineData(-600, 800)] // Negative width
    [InlineData(600, -800)] // Negative height
    [InlineData(0, 800)]    // Zero width
    [InlineData(600, 0)]    // Zero height
    public void ValidateDocument_InvalidPhotoDimensions_ThrowsException(int width, int height)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA", 
            new DateTime(2030, 1, 1),  "OPTIONAL",  "JPEG", "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8")
        {
            PhotoDimensions = (width, height)
        });
    }
}