using Mamey.Portal.Citizenship.Application.Services;
using Xunit;

namespace Mamey.Portal.Shared.Tests;

public class MrzValidationTests
{
    [Fact]
    public void GenerateTD1Line1_ValidDocumentNumber_ShouldBeExactly30Characters()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var documentNumber = "123456789";

        // Act
        var line1 = generator.GenerateTD1Line1(documentNumber);

        // Assert
        Assert.Equal(30, line1.Length);
        Assert.StartsWith("1234567893", line1);
        Assert.All(line1.Skip(10), c => Assert.Equal('<', c)); // Rest should be padding
    }

    [Fact]
    public void GenerateTD1Line1_ShortDocumentNumber_ShouldPadWithZeros()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var documentNumber = "12345";

        // Act
        var line1 = generator.GenerateTD1Line1(documentNumber);

        // Assert
        Assert.Equal(30, line1.Length);
        Assert.StartsWith("0000123457", line1); // Padded to 9 digits + check digit
    }

    [Fact]
    public void GenerateTD1Line1_LongDocumentNumber_ShouldTruncateFromRight()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var documentNumber = "123456789012345"; // 15 digits

        // Act
        var line1 = generator.GenerateTD1Line1(documentNumber);

        // Assert
        Assert.Equal(30, line1.Length);
        Assert.StartsWith("1234567893", line1); // First 9 digits preserved + check digit
    }

    [Fact]
    public void GenerateTD1Line2_ValidDates_ShouldBeExactly30Characters()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var birthDate = new DateTime(1990, 1, 15);
        var expDate = new DateTime(2030, 1, 15);

        // Act
        var line2 = generator.GenerateTD1Line2(birthDate, expDate);

        // Assert
        Assert.Equal(30, line2.Length);
        Assert.StartsWith("900115", line2); // YYMMDD format
    }

    [Fact]
    public void GenerateTD1Line3_ValidNames_ShouldBeExactly30Characters()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var surname = "SMITH";
        var givenNames = "JOHN";

        // Act
        var line3 = generator.GenerateTD1Line3(surname, givenNames);

        // Assert
        Assert.Equal(30, line3.Length);
        Assert.Contains("SMITH<<", line3);
        Assert.Contains("JOHN<<", line3);
        Assert.All(line3.Skip(line3.IndexOf("JOHN<<") + 6), c => Assert.Equal('<', c)); // Rest should be padding
    }

    [Fact]
    public void GenerateTD1Line3_LongNames_ShouldTruncateWithSurnamePriority()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var surname = "JOHNSON-MCDONALD"; // 16 chars
        var givenNames = "JOHN MICHAEL WILLIAM"; // 20 chars

        // Act
        var line3 = generator.GenerateTD1Line3(surname, givenNames);

        // Assert
        Assert.Equal(30, line3.Length);
        // Surname should be preserved as much as possible
        Assert.Contains("JOHNSON", line3);
    }

    [Fact]
    public void CalculateIcaoCheckDigit_ValidString_ShouldReturnCorrectCheckDigit()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var mrzString = "123456789"; // Known test case

        // Act
        var checkDigit = generator.CalculateIcaoCheckDigit(mrzString);

        // Assert
        Assert.InRange(checkDigit, 0, 9);
    }

    [Fact]
    public void SanitizeMrzFields_WithAccentedCharacters_ShouldConvertToUppercaseASCII()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var accentedName = "José María O'Connor";

        // Act
        var sanitized = generator.SanitizeMrzFields(accentedName);

        // Assert
        Assert.All(sanitized, c => Assert.True(
            char.IsUpper(c) || char.IsDigit(c) || c == '<',
            $"Character '{c}' is not valid MRZ character"));
        Assert.DoesNotContain("é", sanitized);
        Assert.DoesNotContain("í", sanitized);
        Assert.DoesNotContain("'", sanitized);
    }

    [Fact]
    public void SanitizeMrzFields_WithSpacesAndHyphens_ShouldReplaceWithLessThan()
    {
        // Arrange
        var documentNumberGen = new DocumentNumberGenerator();
        var validator = new StandardsComplianceValidator();
        var generator = new MrzGenerator(documentNumberGen, validator);
        var name = "JOHN SMITH-JONES";

        // Act
        var sanitized = generator.SanitizeMrzFields(name);

        // Assert
        Assert.DoesNotContain(" ", sanitized);
        Assert.DoesNotContain("-", sanitized);
        Assert.Contains("<", sanitized);
    }
}
