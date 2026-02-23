using Mamey.Security;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Core;

/// <summary>
/// Comprehensive tests for Md5 class covering all scenarios.
/// </summary>
public class Md5Tests
{
    private readonly IMd5 _md5;

    public Md5Tests()
    {
        _md5 = new Md5();
    }

    #region Happy Paths

    [Fact]
    public void Calculate_String_ShouldCalculateMd5Successfully()
    {
        // Arrange
        var value = "Hello, World!";

        // Act
        var hash = _md5.Calculate(value);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        hash.Length.ShouldBe(32); // MD5 produces 32 hex characters
    }

    [Fact]
    public void Calculate_Stream_ShouldCalculateMd5Successfully()
    {
        // Arrange
        var data = "Hello, World!";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

        // Act
        var hash = _md5.Calculate(stream);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        hash.Length.ShouldBe(32);
    }

    [Fact]
    public void Calculate_EmptyString_ShouldCalculateMd5Successfully()
    {
        // Arrange
        var value = "";

        // Act
        var hash = _md5.Calculate(value);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        hash.Length.ShouldBe(32);
    }

    [Fact]
    public void Calculate_LargeData_ShouldCalculateMd5Successfully()
    {
        // Arrange
        var value = TestDataGenerator.GenerateLargeString();

        // Act
        var hash = _md5.Calculate(value);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        hash.Length.ShouldBe(32);
    }

    [Fact]
    public void Calculate_ConsistentHashing_ShouldProduceSameHash()
    {
        // Arrange
        var value = "Test data for consistent hashing";

        // Act
        var hash1 = _md5.Calculate(value);
        var hash2 = _md5.Calculate(value);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Fact]
    public void Calculate_Stream_ConsistentHashing_ShouldProduceSameHash()
    {
        // Arrange
        var data = "Test data";
        var stream1 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
        var stream2 = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

        // Act
        var hash1 = _md5.Calculate(stream1);
        var hash2 = _md5.Calculate(stream2);

        // Assert
        hash1.ShouldBe(hash2);
    }

    [Fact]
    public void Calculate_DifferentInputs_ShouldProduceDifferentHashes()
    {
        // Arrange
        var value1 = "Test data 1";
        var value2 = "Test data 2";

        // Act
        var hash1 = _md5.Calculate(value1);
        var hash2 = _md5.Calculate(value2);

        // Assert
        hash1.ShouldNotBe(hash2);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Calculate_NullString_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? value = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _md5.Calculate(value!));
    }

    [Fact]
    public void Calculate_NullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        Stream? stream = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _md5.Calculate(stream!));
    }

    [Fact]
    public void Calculate_DisposedStream_ShouldThrowException()
    {
        // Arrange
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test data"));
        stream.Dispose();

        // Act & Assert
        Should.Throw<Exception>(() => _md5.Calculate(stream));
    }

    #endregion
}



