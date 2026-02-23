using Mamey.AmvvaStandards.Tests.DataAttributes;
namespace Mamey.AmvvaStandards.Tests.Validators;


public class DriversLicenseCardTests
{
    [Theory]
    [ClassData(typeof(DriverLicenseCardTestData))]
    public void ValidateDriverLicenseCard_VariousScenarios(
        DriverLicenseCard card,
        bool expectedValid,
        string expectedMessageSubstring)
    {
        // Act
        bool isValid = AamvaValidators.ValidateDriverLicenseCard(card, out var error);

        // Assert
        Assert.Equal(expectedValid, isValid);

        if (!string.IsNullOrEmpty(expectedMessageSubstring))
        {
            Assert.Contains(expectedMessageSubstring, error);
        }
    }

    // [Theory]
    // [InlineData(true)]
    // [InlineData(false)]
    // public void ValidateDriverLicenseCard_CommercialAgeCheck(bool isCommercial)
    // {
    //     // This test specifically checks rule #6 (Commercial must be age >= 21).
    //     // Arrange
    //     var card = new DriverLicenseCard
    //     {
    //         FamilyName = "Smith",
    //         GivenName = "Marry",
    //         DateOfBirth = isCommercial
    //             ? new System.DateTime(System.DateTime.UtcNow.Year - 30, 1, 1) // 30 y/o => valid
    //             : new System.DateTime(System.DateTime.UtcNow.Year - 16, 1, 1), // 16 y/o => invalid
    //         IssueDate = System.DateTime.UtcNow.AddYears(-1),
    //         ExpirationDate = System.DateTime.UtcNow.AddYears(5),
    //         LicenseOrIdNumber = "ABC12345",
    //         HeightInches = 70,
    //         DocumentDiscriminator = "DOC888",
    //         Revision = CardDesignRevision.AAMVA2013,
    //         IsCommercial = isCommercial
    //     };
    //
    //     // Act
    //     bool isValid = AamvaValidators.ValidateDriverLicenseCard(card, out var error);
    //
    //     // Assert
    //     if (isCommercial)
    //     {
    //         Assert.True(isValid, $"Expected valid but got error: {error}");
    //     }
    //     else
    //     {
    //         // Actually, if isCommercial = false, there's no age check, so it should pass.
    //         // Let's correct that logic: We'll do 2 separate test methods for clarity.
    //
    //         Assert.True(isValid, "Non-commercial doesn't require age >= 21. Should be valid!");
    //     }
    // }
    // Additional scenario-based test for commercial logic:
    [Theory]
    [InlineData(true,21, true)]  // Exactly 21 => should pass if commercial
    [InlineData(true, 20, false)] // 20 => fail if commercial
    [InlineData(true, 30, true)]  // 30 => pass
    [InlineData(false, 20, false)]
    public void ValidateDriverLicenseCard_CommercialAgeCheck(bool isCommercial, int age, bool expectedValid)
    {
        // Arrange
        var birthYear = System.DateTime.UtcNow.Year - age;
        var card = new DriverLicenseCard
        {
            FamilyName = "Test",
            GivenName = "Commercial",
            DateOfBirth = new System.DateTime(birthYear, 1, 1),
            IssueDate = System.DateTime.UtcNow.AddYears(-1),
            ExpirationDate = System.DateTime.UtcNow.AddYears(5),
            LicenseOrIdNumber = "ABC12345",
            HeightInches = 70,
            DocumentDiscriminator = "DOC999",
            Revision = CardDesignRevision.AAMVA2013,
            IsCommercial = isCommercial,
            Jurisdiction = JurisdictionCode.FL,
            PostalCode = "99999"
        };

        // Act
        bool isValid = AamvaValidators.ValidateDriverLicenseCard(card, out var error);
        
        // Assert
        if (isCommercial)
        {
            Assert.True(isValid, $"Expected valid but got error: {error}");
        }
        else
        {
            // Actually, if isCommercial = false, there's no age check, so it should pass.
            // Let's correct that logic: We'll do 2 separate test methods for clarity.

            Assert.True(isValid, "Non-commercial doesn't require age >= 21. Should be valid!");
        }
        Assert.Equal(expectedValid, isValid);
        if (!expectedValid)
        {
            Assert.Contains("Commercial licenses require age 21+.", error);
        }
    }
}