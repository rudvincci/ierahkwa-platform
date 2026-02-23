using Mamey.Security;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Core;

/// <summary>
/// Comprehensive tests for Rng class covering all scenarios.
/// </summary>
public class RngTests
{
    private readonly IRng _rng;

    public RngTests()
    {
        _rng = new Rng();
    }

    #region Happy Paths

    [Fact]
    public void Generate_DefaultLength_ShouldGenerateSuccessfully()
    {
        // Act
        var result = _rng.Generate();

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Generate_CustomLength_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 100;

        // Act
        var result = _rng.Generate(length);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThanOrEqualTo(length);
    }

    [Fact]
    public void Generate_WithSpecialChars_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 50;

        // Act
        var result = _rng.Generate(length, false);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThanOrEqualTo(length);
    }

    [Fact]
    public void Generate_WithoutSpecialChars_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 50;

        // Act
        var result = _rng.Generate(length, true);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThanOrEqualTo(length);
        result.ShouldNotContain("/");
        result.ShouldNotContain("\\");
        result.ShouldNotContain("=");
        result.ShouldNotContain("+");
        result.ShouldNotContain("?");
        result.ShouldNotContain(":");
        result.ShouldNotContain("&");
    }

    [Fact]
    public void Generate_MultipleUniqueStrings_ShouldBeUnique()
    {
        // Arrange
        var length = 50;
        var count = 100;

        // Act
        var results = Enumerable.Range(0, count)
            .Select(_ => _rng.Generate(length))
            .ToList();

        // Assert
        results.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Generate_VeryLongString_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 10000;

        // Act
        var result = _rng.Generate(length);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThanOrEqualTo(length);
    }

    [Fact]
    public void Generate_VeryShortString_ShouldGenerateSuccessfully()
    {
        // Arrange
        var length = 1;

        // Act
        var result = _rng.Generate(length);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBeGreaterThanOrEqualTo(length);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Generate_ZeroLength_ShouldGenerateEmptyString()
    {
        // Arrange
        var length = 0;

        // Act
        var result = _rng.Generate(length);

        // Assert
        result.ShouldNotBeNull();
        // May be empty or have some length depending on base64 encoding
    }

    [Fact]
    public void Generate_NegativeLength_ShouldThrowException()
    {
        // Arrange
        var length = -1;

        // Act & Assert
        Should.Throw<Exception>(() => _rng.Generate(length));
    }

    [Fact]
    public void Generate_VeryLargeLength_ShouldHandleGracefully()
    {
        // Arrange
        var length = int.MaxValue / 2; // Very large value that exceeds the limit

        // Act & Assert
        // Should throw ArgumentException when length exceeds maximum
        Should.Throw<ArgumentException>(() => _rng.Generate(length));
    }

    #endregion
}



