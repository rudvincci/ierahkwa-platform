using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Attributes;

/// <summary>
/// Comprehensive tests for SecurityAttributeProcessor class covering all scenarios.
/// </summary>
public class SecurityAttributeProcessorTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;
    private readonly SecurityAttributeProcessor _processor;

    public SecurityAttributeProcessorTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
        _processor = new SecurityAttributeProcessor(fixture.SecurityProvider);
    }

    #region Test Classes

    public class TestEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [Encrypted]
        public string? EncryptedProperty { get; set; }

        [Hashed]
        public string? HashedProperty { get; set; }

        public string? RegularProperty { get; set; }
    }

    public class TestEntityMultiple
    {
        [Encrypted]
        public string? Encrypted1 { get; set; }

        [Encrypted]
        public string? Encrypted2 { get; set; }

        [Hashed]
        public string? Hashed1 { get; set; }

        [Hashed]
        public string? Hashed2 { get; set; }
    }

    public class TestEntityNoAttributes
    {
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
    }

    #endregion

    #region Happy Paths - Encrypted Properties

    [Fact]
    public void ProcessEncryptedProperties_ToStorage_ShouldEncrypt()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = "123",
            Name = "Test",
            EncryptedProperty = "sensitive data"
        };

        // Act
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.ToStorage);

        // Assert
        entity.EncryptedProperty.ShouldNotBe("sensitive data");
        entity.EncryptedProperty.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeEncrypted("sensitive data", entity.EncryptedProperty!);
    }

    [Fact]
    public void ProcessEncryptedProperties_FromStorage_ShouldDecrypt()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = "123",
            Name = "Test",
            EncryptedProperty = "sensitive data"
        };
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.ToStorage);
        var encrypted = entity.EncryptedProperty;

        // Act
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.FromStorage);

        // Assert
        entity.EncryptedProperty.ShouldBe("sensitive data");
    }

    [Fact]
    public void ProcessEncryptedProperties_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var original = "sensitive data";
        var entity = new TestEntity
        {
            Id = "123",
            Name = "Test",
            EncryptedProperty = original
        };

        // Act
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.ToStorage);
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.FromStorage);

        // Assert
        entity.EncryptedProperty.ShouldBe(original);
    }

    [Fact]
    public void ProcessEncryptedProperties_MultipleProperties_ShouldEncryptAll()
    {
        // Arrange
        var entity = new TestEntityMultiple
        {
            Encrypted1 = "data1",
            Encrypted2 = "data2"
        };

        // Act
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.ToStorage);

        // Assert
        entity.Encrypted1.ShouldNotBe("data1");
        entity.Encrypted2.ShouldNotBe("data2");
    }

    #endregion

    #region Happy Paths - Hashed Properties

    [Fact]
    public void ProcessHashedProperties_ShouldHash()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = "123",
            Name = "Test",
            HashedProperty = "password123"
        };

        // Act
        _processor.ProcessHashedProperties(entity);

        // Assert
        entity.HashedProperty.ShouldNotBe("password123");
        entity.HashedProperty.ShouldNotBeNullOrEmpty();
        AssertionHelpers.ShouldBeValidHash(entity.HashedProperty!);
    }

    [Fact]
    public void ProcessHashedProperties_MultipleProperties_ShouldHashAll()
    {
        // Arrange
        var entity = new TestEntityMultiple
        {
            Hashed1 = "password1",
            Hashed2 = "password2"
        };

        // Act
        _processor.ProcessHashedProperties(entity);

        // Assert
        entity.Hashed1.ShouldNotBe("password1");
        entity.Hashed2.ShouldNotBe("password2");
        AssertionHelpers.ShouldBeValidHash(entity.Hashed1!);
        AssertionHelpers.ShouldBeValidHash(entity.Hashed2!);
    }

    [Fact]
    public void ProcessHashedProperties_AlreadyHashed_ShouldSkip()
    {
        // Arrange
        var alreadyHashed = _fixture.SecurityProvider.Hash("password123");
        var entity = new TestEntity
        {
            HashedProperty = alreadyHashed
        };

        // Act
        _processor.ProcessHashedProperties(entity);

        // Assert
        entity.HashedProperty.ShouldBe(alreadyHashed);
    }

    #endregion

    #region Happy Paths - All Security Attributes

    [Fact]
    public void ProcessAllSecurityAttributes_ShouldProcessBoth()
    {
        // Arrange
        var entity = new TestEntity
        {
            EncryptedProperty = "sensitive data",
            HashedProperty = "password123"
        };

        // Act
        _processor.ProcessAllSecurityAttributes(entity, ProcessingDirection.ToStorage);

        // Assert
        entity.EncryptedProperty.ShouldNotBe("sensitive data");
        entity.HashedProperty.ShouldNotBe("password123");
        AssertionHelpers.ShouldBeEncrypted("sensitive data", entity.EncryptedProperty!);
        AssertionHelpers.ShouldBeValidHash(entity.HashedProperty!);
    }

    [Fact]
    public void ProcessAllSecurityAttributes_NoAttributes_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntityNoAttributes
        {
            Property1 = "value1",
            Property2 = "value2"
        };

        // Act
        _processor.ProcessAllSecurityAttributes(entity, ProcessingDirection.ToStorage);

        // Assert
        entity.Property1.ShouldBe("value1");
        entity.Property2.ShouldBe("value2");
    }

    #endregion

    #region Happy Paths - Verify Hashed Property

    [Fact]
    public void VerifyHashedProperty_ValidValue_ShouldReturnTrue()
    {
        // Arrange
        var entity = new TestEntity
        {
            HashedProperty = "password123"
        };
        _processor.ProcessHashedProperties(entity);

        // Act
        var isValid = _processor.VerifyHashedProperty(entity, nameof(TestEntity.HashedProperty), "password123");

        // Assert
        isValid.ShouldBeTrue();
    }

    [Fact]
    public void VerifyHashedProperty_InvalidValue_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity
        {
            HashedProperty = "password123"
        };
        _processor.ProcessHashedProperties(entity);

        // Act
        var isValid = _processor.VerifyHashedProperty(entity, nameof(TestEntity.HashedProperty), "wrongpassword");

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void ProcessEncryptedProperties_NullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? entity = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _processor.ProcessEncryptedProperties(entity!, ProcessingDirection.ToStorage));
    }

    [Fact]
    public void ProcessEncryptedProperties_NullEncryptedProperty_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntity
        {
            EncryptedProperty = null
        };

        // Act
        _processor.ProcessEncryptedProperties(entity, ProcessingDirection.ToStorage);

        // Assert
        entity.EncryptedProperty.ShouldBeNull();
    }

    [Fact]
    public void ProcessHashedProperties_NullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? entity = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _processor.ProcessHashedProperties(entity!));
    }

    [Fact]
    public void ProcessHashedProperties_NullHashedProperty_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntity
        {
            HashedProperty = null
        };

        // Act
        _processor.ProcessHashedProperties(entity);

        // Assert
        entity.HashedProperty.ShouldBeNull();
    }

    [Fact]
    public void ProcessHashedProperties_EmptyHashedProperty_ShouldNotThrow()
    {
        // Arrange
        var entity = new TestEntity
        {
            HashedProperty = ""
        };

        // Act
        _processor.ProcessHashedProperties(entity);

        // Assert
        entity.HashedProperty.ShouldBe("");
    }

    [Fact]
    public void ProcessAllSecurityAttributes_NullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? entity = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _processor.ProcessAllSecurityAttributes(entity!, ProcessingDirection.ToStorage));
    }

    [Fact]
    public void VerifyHashedProperty_NullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? entity = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _processor.VerifyHashedProperty(entity!, "Property", "value"));
    }

    [Fact]
    public void VerifyHashedProperty_NullPropertyName_ShouldThrowArgumentException()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Should.Throw<ArgumentException>(() => _processor.VerifyHashedProperty(entity, null!, "value"));
    }

    [Fact]
    public void VerifyHashedProperty_EmptyPropertyName_ShouldThrowArgumentException()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Should.Throw<ArgumentException>(() => _processor.VerifyHashedProperty(entity, "", "value"));
    }

    [Fact]
    public void VerifyHashedProperty_InvalidPropertyName_ShouldThrowArgumentException()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Should.Throw<ArgumentException>(() => _processor.VerifyHashedProperty(entity, "NonExistentProperty", "value"));
    }

    [Fact]
    public void VerifyHashedProperty_PropertyWithoutHashedAttribute_ShouldThrowArgumentException()
    {
        // Arrange
        var entity = new TestEntity();

        // Act & Assert
        Should.Throw<ArgumentException>(() => _processor.VerifyHashedProperty(entity, nameof(TestEntity.RegularProperty), "value"));
    }

    [Fact]
    public void VerifyHashedProperty_EmptyHashedValue_ShouldReturnFalse()
    {
        // Arrange
        var entity = new TestEntity
        {
            HashedProperty = ""
        };

        // Act
        var isValid = _processor.VerifyHashedProperty(entity, nameof(TestEntity.HashedProperty), "password123");

        // Assert
        isValid.ShouldBeFalse();
    }

    #endregion
}



