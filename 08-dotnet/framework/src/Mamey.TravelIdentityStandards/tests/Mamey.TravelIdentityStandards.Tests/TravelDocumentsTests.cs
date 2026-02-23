using System.Reflection;
using Mamey.TravelIdentityStandards.Exceptions;
using Xunit.Sdk;

namespace Mamey.TravelIdentityStandards.Tests;

public class TravelDocumentTests
{
    #region Happy Paths

    [Fact]
    public void ValidateDocument_ValidData_PassesValidation()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        var exception = Record.Exception(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8"));
        Assert.Null(exception); // Validation should pass with no exceptions
    }

    #endregion

    #region Unhappy Paths

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ValidateGivenNames_throws_Exception(string givenNames)
    {
        // Act/Assert
        void PersonAction() => new PersonData(givenNames, "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>((Action)PersonAction);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ValidateSurname_throws_Exception(string surname)
    {
        // Act/Assert
        void PersonAction() => new PersonData("JOHN", surname, "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>((Action)PersonAction);
    }
    

    [Theory]
    [InvalidTravelDocumentDateOfBirthInput]
    public void Constructor_InvalidDateOfBirth_throws_Exception(DateTime dob)
    {
        // Act/Assert


        void PersonAction() => new PersonData("JOHN", "DOE", "M", dob, "USA");
        Assert.Throws<AgeRangeException>((Action)PersonAction);
    }

    [Theory]
    [InvalidDocumentDateRange("2030-01-01", "2010-01-01")]
    public void Document_InvalidDateRange_ThrowsException(DateTime issueDate, DateTime expiryDate)
    {
        var person =  new PersonData("JOHN", "DOE", "M", new DateTime(1978,12,15), "USA");
        

        Assert.Throws<ArgumentException>(() => new PassportDocument(person,
            "123456789", "USA",
            expiryDate, "OPT", "JPEG", "Issuer", "USA", "P",
            new[] { "Hologram", "RFID Chip" }, issueDate));
    }

    #endregion


    [Theory]
    [InlineData("BMP")] // Unsupported format
    [InlineData("")] // Empty format
    [InlineData(null)] // Null format
    public void ValidateDocument_InvalidPhotoFormat_ThrowsException(string format)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");

        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", format, "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8")
        {
            PhotoFormat = format
        });
    }

    

    [Theory]
    [InlineData("")] // Empty issuer name
    [InlineData(null)] // Null issuer name
    public void ValidateDocument_InvalidIssuerName_ThrowsException(string issuerName)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", issuerName, "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8"));
    }

    [Theory]
    [InlineData("US")] // Too short
    [InlineData("USAA")] // Too long
    [InlineData("123")] // Numeric-only code
    [InlineData("")] // Empty
    public void ValidateDocument_InvalidIssuerCode_ThrowsException(string issuerCode)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", issuerCode, "Passport",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8"));
    }

    [Theory]
    [InlineData("")] // Empty document type code
    [InlineData(null)] // Null document type code
    public void ValidateDocument_InvalidDocumentTypeCode_ThrowsException(string documentTypeCode)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");

        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", "USA", documentTypeCode,
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1), "UTF-8"));
    }

    [Fact]
    public void ValidateDocument_InvalidIssueDate_ThrowsException()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, DateTime.MinValue, "UTF-8"));
    }

    [Theory]
    [InlineData("UTF-16")] // Unsupported encoding
    [InlineData("")] // Empty encoding
    [InlineData(null)] // Null encoding
    public void ValidateDocument_InvalidEncoding_ThrowsException(string encoding)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person, "123456789", "USA",
            new DateTime(2030, 1, 1), "OPTIONAL", "JPEG", "United States Passport Agency", "USA", "Passport",
            new[] { "Hologram", "RFID Chip" }, DateTime.MinValue, encoding));
    }

    [Fact]
    public void ElectronicDocument_WithBiometricData_StoresCorrectly()
    {
        var biometricData = new List<string> { "Face", "Fingerprint" };
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA", biometricData);

        var document = new PassportDocument(person,
            "123456789", "USA",
            new DateTime(2030, 1, 1), "OPT", "JPEG", "Issuer", "USA", "P",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1),
            isElectronic: true);

        Assert.True(document.IsElectronic);
        Assert.NotNull(document.PersonData.BiometricData);
        Assert.Contains("Face", document.PersonData.BiometricData);
        Assert.Contains("Fingerprint", document.PersonData.BiometricData);
    }

    [Fact]
    public void ElectronicDocument_MissingBiometricData_ThrowsException()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new PassportDocument(person,
            "123456789", "USA",
            new DateTime(2030, 1, 1), "OPT", "JPEG", "Issuer", "USA", "P",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1),
            isElectronic: true));
    }

    [Fact]
    public void NonElectronicDocument_DoesNotRequireBiometricData()
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
        var document = new PassportDocument(person,
            "123456789", "USA",
            new DateTime(2030, 1, 1), "OPT", "JPEG", "Issuer", "USA", "P",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1),
            isElectronic: false);

        Assert.False(document.IsElectronic);
        Assert.Empty(document.PersonData.BiometricData);
    }

    // [Theory]
    // [InlineData(new string[]{})] // Empty array
    // public void Document_MissingSecurityFeatures_ThrowsException(string[] securityFeatures)
    // {
    //     var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");
    //     Assert.Throws<ArgumentException>(() => new VisaDocument(person,
    //         "987654321", "USA",
    //         new DateTime(2025, 5, 15), "OPT", "JPEG", "Issuer", "USA", "V",
    //         securityFeatures, new DateTime(2020, 1, 1)));
    // }

    [Theory]
    [InlineData("A")]
    [InlineData("X")]
    public void Document_InvalidGender_ThrowsException(string gender)
    {
        var person = new PersonData("JOHN", "DOE", gender, new DateTime(1990, 1, 1), "USA");
        Assert.Throws<ArgumentException>(() => new IdCardTravelDocument(person,
            "123456789", "USA",
            new DateTime(2030, 1, 1), "OPT", "JPEG", "Issuer", "USA", "ID",
            new[] { "Hologram", "RFID Chip" }, new DateTime(2020, 1, 1)));
    }
    


    [Theory]
    [InlineData("USA1")] // Too long
    [InlineData("US")] // Too short
    [InlineData("123")] // Numeric ISO code
    [InlineData("")] // Empty ISO code
    public void Document_InvalidISOCode_ThrowsException(string issuingCountry)
    {
        var person = new PersonData("JOHN", "DOE", "M", new DateTime(1990, 1, 1), "USA");

        Assert.Throws<ArgumentException>(() => new VisaDocument(person,
            "987654321", issuingCountry,
            new DateTime(2025, 5, 15), "OPT", "JPEG", "Issuer", "USA", "V",
            new[] { "UV Light Feature", "Microtext" }, new DateTime(2020, 1, 1)));
    }
}

public class DateInput : DataAttribute
{
    public DateInput()
    {
        Nullable = true;
        ReturnsNull = true;
    }

    public DateInput(int year, int month, int day, int hour, int minute, int second, int millisecond, bool nullable)
    {
        Year = year;
        Month = month;
        Day = day;
        Nullable = nullable;
        ReturnsNull = false;
    }

    public int Year { get; set; } = 1;
    public int Month { get; set; } = 1;
    public int Day { get; set; } = 1;
    public int Hour { get; set; } = 0;
    public int Minute { get; set; } = 0;
    public int Second { get; set; } = 0;
    public int Millisecond { get; set; } = 0;
    public bool Nullable { get; set; } = false;
    public bool ReturnsNull { get; set; } = false;
    
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        if (Nullable)
        {
            if (ReturnsNull)
            {
                yield return new object[] { null };
            }
            DateTime? nullableDate = (DateTime?) new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
            yield return new object[] { nullableDate };
        }
        var date = new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
        yield return new object[] { date };
    }
}
public class InvalidTravelDocumentDateOfBirthInput : DataAttribute
{
    private readonly bool _containsNullable = false;
    public InvalidTravelDocumentDateOfBirthInput()
    {
    }
    
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        var dob1 = new DateTime(2015, 1, 1);
        var dob2 = new DateTime(2020, 1, 1);
        var dob3 = new DateTime(3000, 1, 1);
        yield return new object[] { dob1 };
        yield return new object[] { dob2 };
        yield return new object[] { dob3 };
    }
}
public class InvalidDocumentDateRange : DataAttribute
{
    public InvalidDocumentDateRange(string startDate, string endDate)
    {
        StartDate = DateTime.Parse(startDate);
        EndDate = DateTime.Parse(endDate);
    }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        yield return new object[] { StartDate, EndDate };
    }
}