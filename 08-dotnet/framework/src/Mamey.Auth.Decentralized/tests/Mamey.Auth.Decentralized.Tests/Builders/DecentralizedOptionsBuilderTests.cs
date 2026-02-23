using FluentAssertions;
using Mamey.Auth.Decentralized.Builders;
using Mamey.Auth.Decentralized.Options;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Builders;

/// <summary>
/// Tests for the DecentralizedOptionsBuilder class covering W3C DID 1.1 compliance.
/// </summary>
public class DecentralizedOptionsBuilderTests
{
    #region Happy Path Tests

    [Fact]
    public void SetEnabled_WithTrue_ShouldSetEnabled()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder.SetEnabled(true);

        // Assert
        result.Should().Be(builder);
        builder.Build().EnableDidAuthentication.Should().BeTrue();
    }

    [Fact]
    public void SetEnabled_WithFalse_ShouldSetEnabled()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder.SetEnabled(false);

        // Assert
        result.Should().Be(builder);
        builder.Build().EnableDidAuthentication.Should().BeFalse();
    }

    [Fact]
    public void SetDefaultMethod_WithValidMethod_ShouldSetDefaultMethod()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var method = "web";

        // Act
        var result = builder.SetDefaultMethod(method);

        // Assert
        result.Should().Be(builder);
        builder.Build().DefaultMethod.Should().Be(method);
    }

    [Fact]
    public void SetSupportedMethods_WithValidMethods_ShouldSetSupportedMethods()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var methods = new[] { "web", "key", "ion" };

        // Act
        var result = builder.SetSupportedMethods(methods);

        // Assert
        result.Should().Be(builder);
        builder.Build().SupportedMethods.Should().BeEquivalentTo(methods);
    }

    [Fact]
    public void SetRedisCache_WithValidParameters_ShouldSetRedisCache()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var connectionString = "localhost:6379";
        var expiration = TimeSpan.FromMinutes(30);

        // Act
        var result = builder.SetRedisCache(connectionString, expiration);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.UseRedisCache.Should().BeTrue();
        options.RedisCacheConnection.Should().Be(connectionString);
        options.CacheExpiration.Should().Be(expiration);
    }

    [Fact]
    public void SetPostgreSql_WithValidConnectionString_ShouldSetPostgreSql()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var connectionString = "Host=localhost;Database=test;Username=user;Password=pass";

        // Act
        var result = builder.SetPostgreSql(connectionString);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.UsePostgreSqlStore.Should().BeTrue();
        options.PostgreSqlConnectionString.Should().Be(connectionString);
    }

    [Fact]
    public void SetMongoDb_WithValidParameters_ShouldSetMongoDb()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var connectionString = "mongodb://localhost:27017";
        var databaseName = "test_db";

        // Act
        var result = builder.SetMongoDb(connectionString, databaseName);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.UseMongoStore.Should().BeTrue();
        options.MongoConnectionString.Should().Be(connectionString);
        options.MongoDatabaseName.Should().Be(databaseName);
    }

    [Fact]
    public void SetDefaultCryptoAlgorithm_WithValidAlgorithm_ShouldSetDefaultCryptoAlgorithm()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var algorithm = "Ed25519";

        // Act
        var result = builder.SetDefaultCryptoAlgorithm(algorithm);

        // Assert
        result.Should().Be(builder);
        builder.Build().DefaultCryptoAlgorithm.Should().Be(algorithm);
    }

    [Fact]
    public void SetVerifiableCredentials_WithValidParameters_ShouldSetVerifiableCredentials()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var enableJwt = true;
        var enableJsonLd = true;
        var defaultExpiration = TimeSpan.FromDays(365);

        // Act
        var result = builder.SetVerifiableCredentials(enableJwt, enableJsonLd, defaultExpiration);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.EnableVcJwt.Should().Be(enableJwt);
        options.EnableVcJsonLd.Should().Be(enableJsonLd);
        options.DefaultVcExpiration.Should().Be(defaultExpiration);
    }

    [Fact]
    public void SetIssuerAndAudience_WithValidParameters_ShouldSetIssuerAndAudience()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var issuer = "did:web:issuer.com";
        var audience = "did:web:audience.com";

        // Act
        var result = builder.SetIssuerAndAudience(issuer, audience);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.Issuer.Should().Be(issuer);
        options.Audience.Should().Be(audience);
    }

    [Fact]
    public void Build_WithDefaultValues_ShouldReturnOptions()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<DecentralizedOptions>();
    }

    [Fact]
    public void Build_WithChainedCalls_ShouldReturnOptions()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetEnabled(true)
            .SetDefaultMethod("web")
            .SetSupportedMethods("web", "key")
            .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30))
            .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass")
            .SetMongoDb("mongodb://localhost:27017", "test_db")
            .SetDefaultCryptoAlgorithm("Ed25519")
            .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365))
            .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com")
            .Build();

        // Assert
        result.Should().NotBeNull();
        result.EnableDidAuthentication.Should().BeTrue();
        result.DefaultMethod.Should().Be("web");
        result.SupportedMethods.Should().BeEquivalentTo(new[] { "web", "key" });
        result.UseRedisCache.Should().BeTrue();
        result.RedisCacheConnection.Should().Be("localhost:6379");
        result.CacheExpiration.Should().Be(TimeSpan.FromMinutes(30));
        result.UsePostgreSqlStore.Should().BeTrue();
        result.PostgreSqlConnectionString.Should().Be("Host=localhost;Database=test;Username=user;Password=pass");
        result.UseMongoStore.Should().BeTrue();
        result.MongoConnectionString.Should().Be("mongodb://localhost:27017");
        result.MongoDatabaseName.Should().Be("test_db");
        result.DefaultCryptoAlgorithm.Should().Be("Ed25519");
        result.EnableVcJwt.Should().BeTrue();
        result.EnableVcJsonLd.Should().BeTrue();
        result.DefaultVcExpiration.Should().Be(TimeSpan.FromDays(365));
        result.Issuer.Should().Be("did:web:issuer.com");
        result.Audience.Should().Be("did:web:audience.com");
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetDefaultMethod_WithInvalidMethod_ShouldThrowArgumentException(string invalidMethod)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act & Assert
        var action = () => builder.SetDefaultMethod(invalidMethod);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetSupportedMethods_WithNull_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act & Assert
        var action = () => builder.SetSupportedMethods(null!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetSupportedMethods_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act & Assert
        var action = () => builder.SetSupportedMethods(new string[0]);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetRedisCache_WithInvalidConnectionString_ShouldThrowArgumentException(string invalidConnectionString)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var expiration = TimeSpan.FromMinutes(30);

        // Act & Assert
        var action = () => builder.SetRedisCache(invalidConnectionString, expiration);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetPostgreSql_WithInvalidConnectionString_ShouldThrowArgumentException(string invalidConnectionString)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act & Assert
        var action = () => builder.SetPostgreSql(invalidConnectionString);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetMongoDb_WithInvalidConnectionString_ShouldThrowArgumentException(string invalidConnectionString)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var databaseName = "test_db";

        // Act & Assert
        var action = () => builder.SetMongoDb(invalidConnectionString, databaseName);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetMongoDb_WithInvalidDatabaseName_ShouldThrowArgumentException(string invalidDatabaseName)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var connectionString = "mongodb://localhost:27017";

        // Act & Assert
        var action = () => builder.SetMongoDb(connectionString, invalidDatabaseName);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetDefaultCryptoAlgorithm_WithInvalidAlgorithm_ShouldThrowArgumentException(string invalidAlgorithm)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act & Assert
        var action = () => builder.SetDefaultCryptoAlgorithm(invalidAlgorithm);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetIssuerAndAudience_WithInvalidIssuer_ShouldThrowArgumentException(string invalidIssuer)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var audience = "did:web:audience.com";

        // Act & Assert
        var action = () => builder.SetIssuerAndAudience(invalidIssuer, audience);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void SetIssuerAndAudience_WithInvalidAudience_ShouldThrowArgumentException(string invalidAudience)
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var issuer = "did:web:issuer.com";

        // Act & Assert
        var action = () => builder.SetIssuerAndAudience(issuer, invalidAudience);
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void SetSupportedMethods_WithW3CCompliantMethods_ShouldSetSupportedMethods()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var methods = new[] { "web", "key" };

        // Act
        var result = builder.SetSupportedMethods(methods);

        // Assert
        result.Should().Be(builder);
        builder.Build().SupportedMethods.Should().BeEquivalentTo(methods);
    }

    [Fact]
    public void SetDefaultCryptoAlgorithm_WithW3CCompliantAlgorithm_ShouldSetDefaultCryptoAlgorithm()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var algorithm = "Ed25519";

        // Act
        var result = builder.SetDefaultCryptoAlgorithm(algorithm);

        // Assert
        result.Should().Be(builder);
        builder.Build().DefaultCryptoAlgorithm.Should().Be(algorithm);
    }

    [Fact]
    public void SetVerifiableCredentials_WithW3CCompliantParameters_ShouldSetVerifiableCredentials()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();
        var enableJwt = true;
        var enableJsonLd = true;
        var defaultExpiration = TimeSpan.FromDays(365);

        // Act
        var result = builder.SetVerifiableCredentials(enableJwt, enableJsonLd, defaultExpiration);

        // Assert
        result.Should().Be(builder);
        var options = builder.Build();
        options.EnableVcJwt.Should().Be(enableJwt);
        options.EnableVcJsonLd.Should().Be(enableJsonLd);
        options.DefaultVcExpiration.Should().Be(defaultExpiration);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void SetEnabled_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetEnabled(true)
            .SetDefaultMethod("web")
            .SetSupportedMethods("web", "key");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetDefaultMethod_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetDefaultMethod("web")
            .SetSupportedMethods("web", "key")
            .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30));

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetSupportedMethods_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetSupportedMethods("web", "key")
            .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30))
            .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetRedisCache_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30))
            .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass")
            .SetMongoDb("mongodb://localhost:27017", "test_db");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetPostgreSql_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass")
            .SetMongoDb("mongodb://localhost:27017", "test_db")
            .SetDefaultCryptoAlgorithm("Ed25519");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetMongoDb_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetMongoDb("mongodb://localhost:27017", "test_db")
            .SetDefaultCryptoAlgorithm("Ed25519")
            .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365));

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetDefaultCryptoAlgorithm_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetDefaultCryptoAlgorithm("Ed25519")
            .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365))
            .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetVerifiableCredentials_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365))
            .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com");

        // Assert
        result.Should().Be(builder);
    }

    [Fact]
    public void SetIssuerAndAudience_WithChainedCalls_ShouldReturnBuilder()
    {
        // Arrange
        var builder = new DecentralizedOptionsBuilder();

        // Act
        var result = builder
            .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com");

        // Assert
        result.Should().Be(builder);
    }

    #endregion
}