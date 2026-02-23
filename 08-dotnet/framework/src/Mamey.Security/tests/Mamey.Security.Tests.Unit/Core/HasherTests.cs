using FluentAssertions;
using Mamey.Security.Internals;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Core;

/// <summary>
/// Comprehensive tests for Hasher class covering all scenarios.
/// </summary>
public class HasherTests
{
    private readonly IHasher _hasher;

    public HasherTests()
    {
        _hasher = new Hasher();
    }

    #region Happy Paths

    [Fact]
    public void Hash_String_ShouldHashSuccessfully()
    {
        // Arrange
        var data = "Hello, World!";

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(hash);
    }

    [Fact]
    public void Hash_ByteArray_ShouldHashSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNull();
        hash.Length.ShouldBe(64); // SHA-512 produces 64 bytes
    }

    [Fact]
    public void Hash_EmptyString_ShouldHashSuccessfully()
    {
        // Arrange
        var data = "";

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(hash);
    }

    [Fact]
    public void Hash_LargeData_ShouldHashSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateLargeString();

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(hash);
    }

    [Fact]
    public void Hash_UnicodeString_ShouldHashSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateUnicodeString();

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(hash);
    }

    [Fact]
    public void Hash_SpecialCharacters_ShouldHashSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateStringWithSpecialChars();

        // Act
        var hash = _hasher.Hash(data);

        // Assert
        hash.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(hash);
    }

    [Fact]
    public void Hash_ConsistentHashing_ShouldProduceSameHash()
    {
        // Arrange
        var data = "Test data for consistent hashing";

        // Act
        var hash1 = _hasher.Hash(data);
        var hash2 = _hasher.Hash(data);

        // Assert
        hash1.ShouldBe(hash2);
        AssertionHelpers.ShouldBeConsistentHash(hash1, hash2);
    }

    [Fact]
    public void HashToBytes_String_ShouldHashSuccessfully()
    {
        // Arrange
        var data = "Hello, World!";

        // Act
        var hash = _hasher.HashToBytes(data);

        // Assert
        hash.ShouldNotBeNull();
        hash.Length.ShouldBe(64); // SHA-512 produces 64 bytes
    }

    [Fact]
    public void HashToBytes_ConsistentHashing_ShouldProduceSameHash()
    {
        // Arrange
        var data = "Test data";

        // Act
        var hash1 = _hasher.HashToBytes(data);
        var hash2 = _hasher.HashToBytes(data);

        // Assert
        hash1.ShouldBeEquivalentTo(hash2);
    }

    [Fact]
    public void Hash_DifferentInputs_ShouldProduceDifferentHashes()
    {
        // Arrange
        var data1 = "Test data 1";
        var data2 = "Test data 2";

        // Act
        var hash1 = _hasher.Hash(data1);
        var hash2 = _hasher.Hash(data2);

        // Assert
        hash1.ShouldNotBe(hash2);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void Hash_NullString_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? data = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.Hash(data!));
    }

    [Fact]
    public void Hash_WhitespaceString_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = "   ";

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.Hash(data));
    }

    [Fact]
    public void Hash_NullByteArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[]? data = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.Hash(data!));
    }

    [Fact]
    public void Hash_EmptyByteArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.Hash(data));
    }

    [Fact]
    public void HashToBytes_NullString_ShouldThrowArgumentNullException()
    {
        // Arrange
        string? data = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.HashToBytes(data!));
    }

    [Fact]
    public void HashToBytes_WhitespaceString_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = "   ";

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _hasher.HashToBytes(data));
    }

    #endregion
}



