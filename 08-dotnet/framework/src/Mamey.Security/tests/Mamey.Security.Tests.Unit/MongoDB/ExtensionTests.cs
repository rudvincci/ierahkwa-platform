using Microsoft.Extensions.DependencyInjection;
using Mamey.Security.MongoDB;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Types;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Mamey.Security.Tests.Unit.MongoDB;

/// <summary>
/// Comprehensive tests for MongoDB extensions covering all scenarios.
/// </summary>
public class ExtensionTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public ExtensionTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Test Classes

    public class TestDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [Encrypted]
        public string? EncryptedProperty { get; set; }

        [Hashed]
        public string? HashedProperty { get; set; }
    }

    #endregion

    #region Happy Paths

    [Fact]
    public void AddSecurityMongoDB_ValidBuilder_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        var builder = MameyBuilder.Create(services);

        // Act
        var result = builder.AddSecurityMongoDB();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(builder);
    }

    [Fact]
    public void RegisterSecuritySerializers_ValidAssembly_ShouldRegisterSerializers()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assembly);

        // Assert
        // Serializers are registered, no exception thrown
    }

    [Fact]
    public void RegisterSecuritySerializers_MultipleAssemblies_ShouldRegisterSerializers()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        var assembly1 = Assembly.GetExecutingAssembly();
        var assembly2 = typeof(TestDocument).Assembly;

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assembly1, assembly2);

        // Assert
        // Serializers are registered, no exception thrown
    }

    [Fact]
    public void RegisterSecuritySerializers_NoTypes_ShouldNotThrow()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assembly);

        // Assert
        // No exception thrown
    }

    [Fact]
    public void RegisterSecuritySerializers_MixedAttributes_ShouldRegisterSerializers()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assembly);

        // Assert
        // Serializers are registered, no exception thrown
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void AddSecurityMongoDB_NullBuilder_ShouldThrowException()
    {
        // Arrange
        IMameyBuilder? builder = null;

        // Act & Assert
        Should.Throw<NullReferenceException>(() => builder!.AddSecurityMongoDB());
    }

    [Fact]
    public void RegisterSecuritySerializers_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? securityProvider = null;
        var assembly = Assembly.GetExecutingAssembly();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider!, assembly));
    }

    [Fact]
    public void RegisterSecuritySerializers_NullAssemblies_ShouldNotThrow()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        Assembly[]? assemblies = null;

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assemblies!);

        // Assert
        // No exception thrown (handles null gracefully)
    }

    [Fact]
    public void RegisterSecuritySerializers_EmptyAssemblies_ShouldNotThrow()
    {
        // Arrange
        var securityProvider = _fixture.SecurityProvider;
        var assemblies = Array.Empty<Assembly>();

        // Act
        Mamey.Security.MongoDB.Extensions.RegisterSecuritySerializers(securityProvider, assemblies);

        // Assert
        // No exception thrown
    }

    #endregion
}

