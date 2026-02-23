namespace Mamey.AmvvaStandards.Tests.Validators;

public class IdentificationCardTests
{
    [Theory]
    [InlineData("Doe", "Jane", "ABCDE123", 50, "ABC123DOC", true)]  // Happy path
    [InlineData("", "Jane", "ABCDE123", 50, "ABC123DOC", false)]   // Missing family name
    [InlineData("Doe", "", "ABCDE123", 50, "ABC123DOC", false)]    // Missing given name
    [InlineData("Doe", "Jane", "AB", 50, "ABC123DOC", false)]      // License # too short
    [InlineData("Doe", "Jane", "ABCDE123", 40, "ABC123DOC", false)]// Height out of range
    [InlineData("Doe", "Jane", "ABCDE123", 50, "", false)]         // Missing DocDiscriminator for AAMVA2010+ 
    public void ValidateIdentificationCard_Scenarios(
        string familyName,
        string givenName,
        string idNumber,
        int heightInches,
        string discriminator,
        bool expectedValid)
    {
        // Arrange
        var card = new IdentificationCard
        {
            FamilyName = familyName,
            GivenName = givenName,
            LicenseOrIdNumber = idNumber,
            HeightInches = heightInches,
            DocumentDiscriminator = discriminator,
            Revision = CardDesignRevision.AAMVA2010, // triggers doc discriminator check
            DateOfBirth = new DateTime(2000, 1, 1),
            IssueDate = new DateTime(2022, 1, 1),
            ExpirationDate = new DateTime(2025, 1, 1),
        };

        // Act
        var result = AamvaValidators.ValidateIdentificationCard(card, out var errorMsg);

        // Assert
        Assert.Equal(expectedValid, result);
        if (!result)
        {
            Assert.False(string.IsNullOrEmpty(errorMsg), "Error message should be present for invalid scenario.");
        }
    }
}