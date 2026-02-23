namespace Mamey.AmvvaStandards.Tests.Services;

public class DefaultCardNumberGeneratorTests
{
    [Fact]
    public void GenerateCardNumber_ShouldReturnValidFormat_WhenCommercialIsFalse()
    {
        // Arrange
        var generator = new DefaultCardNumberGenerator();

        // Act
        string result = generator.GenerateCardNumber(IssuingCountry.USA, JurisdictionCode.TX, isCommercial: false);

        // Assert
        // Example format: "TXD20231234560"
        // - StatePrefix ("TX")
        // - "D" if not commercial
        // - YearPart
        // - 6-digit random
        // - 1-digit check
        Assert.NotNull(result);
        Assert.True(result.Length >= 14, "Result length should be at least 14 to accommodate the format.");

        Assert.StartsWith("TXD", result); // Because isCommercial = false -> 'D'
    }

    [Fact]
    public void GenerateCardNumber_ShouldReturnValidFormat_WhenCommercialIsTrue()
    {
        // Arrange
        var generator = new DefaultCardNumberGenerator();

        // Act
        string result = generator.GenerateCardNumber(IssuingCountry.USA, JurisdictionCode.CA, isCommercial: true);

        // Assert
        // Expect something like "CAC2023xxxxxxX"
        Assert.NotNull(result);
        Assert.StartsWith("CAC", result); // Because isCommercial = true -> 'C'
    }

    [Fact]
    public void GenerateCardNumber_ShouldBeUnique_MockDataStoreExample()
    {
        // This is purely illustrative. 
        // In real usage, you'd have an interface IUniqueIdRepository or similar to check uniqueness.
        // We'll show how you'd use Moq to demonstrate uniqueness checks.

        // Suppose we have an interface:
        //   public interface IUniqueIdStore { bool Exists(string cardNumber); void Save(string cardNumber); }
        // And you pass it in constructor or method. For demonstration, we'll just show the approach:

        // var mockStore = new Mock<IUniqueIdStore>();
        // mockStore.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

        // var generator = new DefaultCardNumberGenerator(mockStore.Object);

        // We'll just do a minimal demonstration of generating two different results:
        var generator = new DefaultCardNumberGenerator();

        string first = generator.GenerateCardNumber(IssuingCountry.USA, JurisdictionCode.FL);
        string second = generator.GenerateCardNumber(IssuingCountry.USA, JurisdictionCode.FL);

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void GenerateCardNumber_ShouldContainValidCheckDigit()
    {
        // Arrange
        var generator = new DefaultCardNumberGenerator();

        // Act
        var cardNumber = generator.GenerateCardNumber(IssuingCountry.USA, JurisdictionCode.NY);

        // The last digit is the check digit. We'll verify by re-computing.
        Assert.True(cardNumber.Length > 1, "Generated card number should have at least 2 characters.");

        // Extract check digit
        int checkDigit = int.Parse(cardNumber[^1].ToString());
        // Extract partial (without the check digit)
        string partial = cardNumber[..^1];

        // Recompute the check digit
        int sum = 0;
        foreach (char c in partial)
        {
            sum += c;
        }

        int expected = sum % 10;

        Assert.Equal(expected, checkDigit);
    }
}