using Mamey.TravelIdentityStandards.MachineReadableZone;

namespace Mamey.TravelIdentityStandards.Tests;

public class VisaMrzGeneratorTests
{
    #region Happy Paths
    [Fact]
    public void TD2MRZGenerator_CompositeChecksum_ValidatesCorrectly()
    {
        var data = new MrzData("987654321", "USA", "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "DOE",
            "JOHN", "M", "1234");

        var mrz = (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data);
        Assert.NotNull(mrz);
        Assert.Equal(typeof(VisaMrz), mrz.GetType());

        // Combine Line 1 and Line 2 for composite checksum

        var compositeString =  string.Concat(mrz).TrimEnd('<');
        var compositeStringWithoutChecksum = compositeString.Substring(0, mrz.Line2.TrimEnd('<').Length-1);

        
        // Calculate the checksum
        var checkDigit = MRZUtils.CalculateCheckDigit(compositeStringWithoutChecksum);

        // Validate the checksum
        Assert.EndsWith(checkDigit.ToString(), compositeString);
    }
    [Fact]
    public void TD2MRZGenerator_ValidData_ReturnsCorrectFormat()
    {
        var data = new MrzData("987654321", "USA", "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "SMITH",
            "JANE", "M", "123456");

        var mrz = (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data);

        Assert.Equal(typeof(VisaMrz), mrz.GetType());

        // Line 1: Document type, issuing country, document number, surname, given names
        Assert.Matches(@"^P<USA987654321[0-9]SMITH<<JANE<<<<<<<<<<$", mrz.Line1);

        // Line 2: DOB, gender, expiry date, nationality, optional data, checksum
        Assert.Matches(@"^[0-9]{6}[0-9][MF][0-9]{6}[0-9]{3}[0-9]$", mrz.Line2);
    }
    [Fact]
    public void TD2MRZGenerator_NullOptionalData_HandlesGracefully()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "SMITH",
            "JANE", "M", "");

        var mrz = (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data);

        Assert.Equal(typeof(VisaMrz), mrz.GetType());
        Assert.DoesNotContain("NULL", mrz.ToString());
    }
    #endregion
    
    #region Unhappy Paths
    [Fact]
    public void TD2MRZGenerator_InvalidDocumentNumber_ThrowsException()
    {
        var data = new MrzData("TOOLONG123", "USA", "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "SMITH",
            "JANE", "M", "OPTDATA");

        Assert.Throws<ArgumentException>(() => (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data));
    }
    [Theory]
    [InlineData("INVALID")]
    public void TD2MRZGenerator_InvalidIssuingCountry_ThrowsException(string issuingCountry)
    {
        
        var data = new MrzData("123456789", issuingCountry, "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "SMITH",
            "JANE", "M", "OPTDATA");
        

        Assert.Throws<ArgumentException>(() => (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data));
    }
    [Fact]
    public void TD2MRZGenerator_InvalidOptionalDataLength_ThrowsException()
    {
        var data = new MrzData("123456789", "USA", "USA",
            new DateTime(1985, 5, 15), new DateTime(2025, 5, 15), "SMITH",
            "JANE", "M", "TOOLONGOPTIONALDATA");
        

        Assert.Throws<ArgumentException>(() => (VisaMrz) MrzGenerator.GenerateMrz(DocumentType.Visa, data));
    }
    
    #endregion
}