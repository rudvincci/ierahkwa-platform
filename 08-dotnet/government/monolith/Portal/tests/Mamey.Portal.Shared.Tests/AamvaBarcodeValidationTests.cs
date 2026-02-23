using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Services;
using Xunit;

namespace Mamey.Portal.Shared.Tests;

public class AamvaBarcodeValidationTests
{
    // Note: Full AAMVA card validation tests require complex card setup.
    // These tests focus on the validation and truncation logic that can be tested independently.

    [Fact]
    public void TruncateAamvaField_LongName_ShouldTruncateFromRight()
    {
        // Arrange
        var validator = new StandardsComplianceValidator();
        var longName = "JOHNSON-MCDONALD-WILLIAMS-SMITH-JONES-BROWN"; // 45 characters

        // Act
        var truncated = validator.TruncateAamvaField(longName, 35, FieldType.Name);

        // Assert
        Assert.Equal(35, truncated.Length);
        Assert.StartsWith("JOHNSON-MCDONALD-WILLIAMS-SMITH", truncated);
    }

    [Fact]
    public void TruncateAamvaField_LongAddress_ShouldTruncateFromRight()
    {
        // Arrange
        var validator = new StandardsComplianceValidator();
        var longAddress = "1234 MAIN STREET APARTMENT 5B BUILDING C"; // 42 characters

        // Act
        var truncated = validator.TruncateAamvaField(longAddress, 35, FieldType.Address);

        // Assert
        Assert.Equal(35, truncated.Length);
        Assert.StartsWith("1234 MAIN STREET APARTMENT 5B", truncated);
    }

    [Fact]
    public void SanitizeAamvaField_WithAccentedCharacters_ShouldConvertToASCII()
    {
        // Arrange
        var validator = new StandardsComplianceValidator();
        var accentedName = "José María O'Connor";

        // Act
        var sanitized = validator.SanitizeAamvaField(accentedName);

        // Assert
        Assert.DoesNotContain("é", sanitized);
        Assert.DoesNotContain("í", sanitized);
        Assert.Contains("Jose", sanitized);
        Assert.Contains("Maria", sanitized);
    }

    [Fact]
    public void SanitizeAamvaField_WithControlCharacters_ShouldRemove()
    {
        // Arrange
        var validator = new StandardsComplianceValidator();
        var textWithControlChars = "JOHN\x00SMITH\x01\x02";

        // Act
        var sanitized = validator.SanitizeAamvaField(textWithControlChars);

        // Assert
        Assert.DoesNotContain('\0', sanitized);
        Assert.DoesNotContain('\x01', sanitized);
        Assert.DoesNotContain('\x02', sanitized);
        Assert.Contains("JOHN", sanitized);
        Assert.Contains("SMITH", sanitized);
    }
}
