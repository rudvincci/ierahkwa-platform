using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards.Tests;

public class IdCardMrzGeneratorTests
{
    [Fact]
    public void TD1MRZGenerator_CompositeChecksum_ValidatesCorrectly()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1990, 1, 1), new DateTime(2030, 1, 1), "DOE",
            "JOHN", "M", "OPT1");

        var mrz = (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data);

        Assert.NotNull(mrz);

        // Line 1, Line 2, and Line 3 form the composite input
        string compositeString = string.Concat(mrz.Line1, mrz.Line2, mrz.Line3).TrimEnd('<'); // Remove any padding


        // Calculate the checksum
        var checkDigit = MRZUtils.CalculateCheckDigit(compositeString);


        // Validate the checksum
        Assert.EndsWith(checkDigit.ToString(), mrz.Line2);
    }

    [Fact]
    public void TD1MRZGenerator_ValidData_ReturnsCorrectFormat()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1990, 1, 1), new DateTime(2030, 1, 1), "DOE",
            "JOHN", "M", "OPTIONAL");

        var mrz = (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data);

        Assert.Equal(typeof(IdCardMrz), mrz.GetType());
        
        // Line 1: Document type, issuing country, document number, nationality, optional data
        Assert.Matches(MRZUtils.Td1Line1Regex, mrz.Line1);

        // Line 2: DOB, gender, expiry date, optional data
        Assert.Matches(MRZUtils.Td1Line2Regex, mrz.Line2);

        // Line 3: Surname, given names
        Assert.Matches(MRZUtils.TdLine3Regex, mrz.Line3);
    }

    [Fact]
    public void TD1MRZGenerator_InvalidDocumentNumber_ThrowsException()
    {
        var data = new MrzData("TOOLONG123", "USA", "USA",
            DateTime.MinValue, DateTime.MinValue, "DOE",
            "JOHN", "M", "OPTIONAL");

        Assert.Throws<ArgumentException>(() => (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data));
    }

    [Fact]
    public void TD1MRZGenerator_InvalidIssuingCountry_ThrowsException()
    {
        var data = new MrzData("123456789", "INVALID", "USA",
            DateTime.MinValue, DateTime.MinValue, "DOE",
            "JOHN", "M", "OPTIONAL");

        Assert.Throws<ArgumentException>(() => (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data));
    }

    [Fact]
    public void TD1MRZGenerator_NullOptionalData_HandlesGracefully()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1990, 1, 1), new DateTime(2030, 1, 1), "DOE",
            "JOHN", "M", "");

        var mrz = (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data);

        Assert.Equal(typeof(IdCardMrz), mrz.GetType());
        Assert.DoesNotContain("NULL", mrz.ToString());
    }

    [Fact]
    public void TD1MRZGenerator_InvalidOptionalDataLength_ThrowsException()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1990, 1, 1), new DateTime(2030, 1, 1), "DOE",
            "JOHN", "M", "TOOLONGOPTIONALDATA");
        Assert.Throws<ArgumentException>(() => (IdCardMrz) MrzGenerator.GenerateMrz(DocumentType.IdCard, data));
    }

    [Fact]
    public void IdCardMRZGeneration_ValidData_ReturnsCorrectMRZ()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        var document = new IdCardTravelDocument(person,
            "123456789", "USA", 
            new DateTime(2030, 1, 1), "OPT",  "JPEG", "Issuer", "USA", "ID",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1));

        Assert.Equal(typeof(IdCardMrz), document.Mrz.GetType());
        Assert.True(document.Mrz.Line1.Length == 30);
        Assert.True(document.Mrz.Line2.Length == 30);
        Assert.True(document.Mrz.Line3.Length == 30);
    }

    [Fact]
    public void PassportMRZGeneration_InvalidOptionalData_ThrowsException()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new PassportDocument(person,
            "123456789", "USA", 
             new DateTime(2030, 1, 1), "TOOLONGOPTIONALDATA", "JPEG", "Issuer", "USA", "P",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1)));
    }

    [Fact]
    public void VisaMRZGeneration_ValidData_ReturnsCorrectMRZ()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        var document = new VisaDocument(person,
            "987654321", "USA", 
            new DateTime(2025, 5, 15),  "1234", "PNG", "Issuer", "USA", "V",
            new[] { "UV Light Feature", "Microtext" }, new DateTime(2020, 1, 1));

        Assert.Equal(typeof(VisaMrz), document.Mrz.GetType());
        Assert.True(document.Mrz.Line1.Length == 36);
        Assert.True(document.Mrz.Line2.Length == 36);
    }

    [Fact]
    public void IdCardMRZGeneration_InvalidDateOfBirth_ThrowsException()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person,
            "123456789", "USA", 
            new DateTime(2030, 1, 1),  "OPT",  "JPEG", "Issuer", "USA", "ID",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1)));
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
             new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8")
        {
            PhotoDimensions = (width, height)
        });
    }
}
