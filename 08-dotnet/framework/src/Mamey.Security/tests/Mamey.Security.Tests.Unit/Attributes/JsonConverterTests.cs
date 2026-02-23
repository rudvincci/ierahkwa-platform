using System.Text.Json;
using Mamey.Security;
using Mamey.Security.JsonConverters;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Attributes;

/// <summary>
/// Comprehensive tests for JSON converters covering all scenarios.
/// </summary>
public class JsonConverterTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;
    private readonly JsonSerializerOptions _options;

    public JsonConverterTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
        _options = new JsonSerializerOptions();
        _options.Converters.Add(new EncryptedJsonConverter(fixture.SecurityProvider));
        _options.Converters.Add(new HashedJsonConverter(fixture.SecurityProvider));
    }

    #region Test Classes

    public class TestObject
    {
        public string? RegularProperty { get; set; }
        public string? EncryptedProperty { get; set; }
        public string? HashedProperty { get; set; }
    }

    #endregion

    #region Happy Paths - EncryptedJsonConverter

    [Fact]
    public void EncryptedJsonConverter_Serialize_ShouldEncrypt()
    {
        // Arrange
        var value = "sensitive data";

        // Act
        var json = JsonSerializer.Serialize(value, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBe(value);
        json.ShouldNotContain("sensitive data");
    }

    [Fact]
    public void EncryptedJsonConverter_Deserialize_ShouldDecrypt()
    {
        // Arrange
        var original = "sensitive data";
        var json = JsonSerializer.Serialize(original, _options);

        // Act
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void EncryptedJsonConverter_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var original = "sensitive data";

        // Act
        var json = JsonSerializer.Serialize(original, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBe(original);
    }

    [Fact]
    public void EncryptedJsonConverter_NullValue_ShouldHandleNull()
    {
        // Arrange
        string? value = null;

        // Act
        var json = JsonSerializer.Serialize(value, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBeNull();
    }

    [Fact]
    public void EncryptedJsonConverter_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var value = "";

        // Act
        var json = JsonSerializer.Serialize(value, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBe(value);
    }

    [Fact]
    public void EncryptedJsonConverter_ComplexObject_ShouldEncryptProperty()
    {
        // Arrange
        var obj = new TestObject
        {
            RegularProperty = "regular",
            EncryptedProperty = "sensitive"
        };

        // Act
        var json = JsonSerializer.Serialize(obj, _options);
        var deserialized = JsonSerializer.Deserialize<TestObject>(json, _options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.RegularProperty.ShouldBe("regular");
        deserialized.EncryptedProperty.ShouldBe("sensitive");
    }

    #endregion

    #region Happy Paths - HashedJsonConverter

    [Fact]
    public void HashedJsonConverter_Serialize_ShouldHash()
    {
        // Arrange
        var value = "password123";
        var hashedOptions = new JsonSerializerOptions();
        hashedOptions.Converters.Add(new HashedJsonConverter(_fixture.SecurityProvider));

        // Act
        var json = JsonSerializer.Serialize(value, hashedOptions);
        var deserialized = JsonSerializer.Deserialize<string>(json, hashedOptions);

        // Assert
        deserialized.ShouldNotBe(value);
        AssertionHelpers.ShouldBeValidHash(deserialized!);
    }

    [Fact]
    public void HashedJsonConverter_Deserialize_ShouldReturnStoredHash()
    {
        // Arrange
        var value = "password123";
        var hashedOptions = new JsonSerializerOptions();
        hashedOptions.Converters.Add(new HashedJsonConverter(_fixture.SecurityProvider));
        var json = JsonSerializer.Serialize(value, hashedOptions);

        // Act
        var deserialized = JsonSerializer.Deserialize<string>(json, hashedOptions);

        // Assert
        deserialized.ShouldNotBe(value);
        AssertionHelpers.ShouldBeValidHash(deserialized!);
    }

    [Fact]
    public void HashedJsonConverter_NullValue_ShouldHandleNull()
    {
        // Arrange
        string? value = null;

        // Act
        var json = JsonSerializer.Serialize(value, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBeNull();
    }

    [Fact]
    public void HashedJsonConverter_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var value = "";

        // Act
        var json = JsonSerializer.Serialize(value, _options);
        var deserialized = JsonSerializer.Deserialize<string>(json, _options);

        // Assert
        deserialized.ShouldBe(value);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void EncryptedJsonConverter_NullSecurityProvider_ShouldThrowException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new EncryptedJsonConverter(provider!));
    }

    [Fact]
    public void HashedJsonConverter_NullSecurityProvider_ShouldThrowException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new HashedJsonConverter(provider!));
    }

    [Fact]
    public void EncryptedJsonConverter_CorruptedData_ShouldThrowException()
    {
        // Arrange
        var corruptedJson = "\"corrupted encrypted data\"";

        // Act & Assert
        Should.Throw<Exception>(() => JsonSerializer.Deserialize<string>(corruptedJson, _options));
    }

    #endregion
}



