using FluentAssertions;
using Mamey.Auth.Decentralized;
using Mamey.Auth.Decentralized.Options;
using Mamey.Auth.Decentralized.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Extensions;

/// <summary>
/// Tests for the DecentralizedExtensions class covering W3C DID 1.1 compliance.
/// </summary>
public class DecentralizedExtensionsTests
{
    #region Happy Path Tests

    [Fact]
    public void AddDecentralizedAuth_WithValidServices_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new Dictionary<string, string>
        {
            ["Decentralized:EnableDidAuthentication"] = "true",
            ["Decentralized:EnableDidValidation"] = "true",
            ["Decentralized:EnableCaching"] = "true",
            ["Decentralized:DidHeaderName"] = "X-DID",
            ["Decentralized:DefaultCacheDuration"] = "00:15:00",
            ["Decentralized:DidDocumentCacheDuration"] = "01:00:00",
            ["Decentralized:VerificationMethodCacheDuration"] = "02:00:00",
            ["Decentralized:ServiceEndpointCacheDuration"] = "02:00:00",
            ["Decentralized:MaxCacheSize"] = "10000",
            ["Decentralized:CacheCleanupInterval"] = "00:30:00",
            ["Decentralized:SupportedDidMethods"] = "web,key",
            ["Decentralized:SupportedVerificationMethodTypes"] = "Ed25519VerificationKey2020,RsaVerificationKey2018,Secp256k1VerificationKey2018",
            ["Decentralized:SupportedServiceTypes"] = "DIDCommMessaging,LinkedDomains,DIDCommV2",
            ["Decentralized:MaxDidLength"] = "2048",
            ["Decentralized:MaxDidDocumentSize"] = "1048576",
            ["Decentralized:MaxVerificationMethods"] = "100",
            ["Decentralized:MaxServiceEndpoints"] = "100",
            ["Decentralized:ResolutionTimeout"] = "00:00:30",
            ["Decentralized:ValidationTimeout"] = "00:00:10",
            ["Decentralized:RetryCount"] = "3",
            ["Decentralized:RetryDelay"] = "00:00:01",
            ["Decentralized:EnableDetailedLogging"] = "false",
            ["Decentralized:EnablePerformanceMetrics"] = "true",
            ["Decentralized:MetricsCollectionInterval"] = "00:05:00",
            ["Decentralized:HealthCheckTimeout"] = "00:00:05",
            ["Decentralized:HealthCheckInterval"] = "00:01:00",
            ["Decentralized:CircuitBreakerFailureThreshold"] = "5",
            ["Decentralized:CircuitBreakerTimeout"] = "00:01:00",
            ["Decentralized:CircuitBreakerRetryTimeout"] = "00:00:30",
            ["Decentralized:RateLimitRequestsPerMinute"] = "1000",
            ["Decentralized:RateLimitBurstSize"] = "100",
            ["Decentralized:RateLimitWindowSize"] = "00:01:00"
        };

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration!)
            .Build();

        // Act
        var result = services.AddDecentralizedAuth(configurationRoot);

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithAction_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(options =>
        {
            options.EnableDidAuthentication = true;
            options.EnableDidValidation = true;
            options.EnableCaching = true;
            options.DidHeaderName = "X-DID";
            options.DefaultCacheDuration = TimeSpan.FromMinutes(15);
            options.DidDocumentCacheDuration = TimeSpan.FromHours(1);
            options.VerificationMethodCacheDuration = TimeSpan.FromHours(2);
            options.ServiceEndpointCacheDuration = TimeSpan.FromHours(2);
            options.MaxCacheSize = 10000;
            options.CacheCleanupInterval = TimeSpan.FromMinutes(30);
            options.SupportedDidMethods = new List<string> { "web", "key" };
            options.SupportedVerificationMethodTypes = new List<string>
            {
                "Ed25519VerificationKey2020",
                "RsaVerificationKey2018",
                "Secp256k1VerificationKey2018"
            };
            options.SupportedServiceTypes = new List<string>
            {
                "DIDCommMessaging",
                "LinkedDomains",
                "DIDCommV2"
            };
            options.MaxDidLength = 2048;
            options.MaxDidDocumentSize = 1048576;
            options.MaxVerificationMethods = 100;
            options.MaxServiceEndpoints = 100;
            options.ResolutionTimeout = TimeSpan.FromSeconds(30);
            options.ValidationTimeout = TimeSpan.FromSeconds(10);
            options.RetryCount = 3;
            options.RetryDelay = TimeSpan.FromSeconds(1);
            options.EnableDetailedLogging = false;
            options.EnablePerformanceMetrics = true;
            options.MetricsCollectionInterval = TimeSpan.FromMinutes(5);
            options.HealthCheckTimeout = TimeSpan.FromSeconds(5);
            options.HealthCheckInterval = TimeSpan.FromMinutes(1);
            options.CircuitBreakerFailureThreshold = 5;
            options.CircuitBreakerTimeout = TimeSpan.FromMinutes(1);
            options.CircuitBreakerRetryTimeout = TimeSpan.FromSeconds(30);
            options.RateLimitRequestsPerMinute = 1000;
            options.RateLimitBurstSize = 100;
            options.RateLimitWindowSize = TimeSpan.FromMinutes(1);
        });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithBuilder_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(builder =>
        {
            builder
                .SetEnabled(true)
                .SetDefaultMethod("web")
                .SetSupportedMethods("web", "key")
                .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30))
                .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass")
                .SetMongoDb("mongodb://localhost:27017", "test_db")
                .SetDefaultCryptoAlgorithm("Ed25519")
                .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365))
                .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com");
        });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithDefaultConfiguration_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth();

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    #endregion

    #region Unhappy Path Tests

    [Fact]
    public void AddDecentralizedAuth_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => DecentralizedExtensions.AddDecentralizedAuth(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddDecentralizedAuth_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddDecentralizedAuth((IConfiguration)null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddDecentralizedAuth_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddDecentralizedAuth((Action<DecentralizedOptions>)null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddDecentralizedAuth_WithNullBuilderAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddDecentralizedAuth((Action<IDecentralizedOptionsBuilder>)null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void AddDecentralizedAuth_WithW3CCompliantConfiguration_ShouldAddServices()
    {
        // Arrange - Configuration compliant with W3C DID 1.1 spec
        var services = new ServiceCollection();
        var configuration = new Dictionary<string, string>
        {
            ["Decentralized:EnableDidAuthentication"] = "true",
            ["Decentralized:EnableDidValidation"] = "true",
            ["Decentralized:EnableCaching"] = "true",
            ["Decentralized:DidHeaderName"] = "X-DID",
            ["Decentralized:DefaultCacheDuration"] = "00:15:00",
            ["Decentralized:DidDocumentCacheDuration"] = "01:00:00",
            ["Decentralized:VerificationMethodCacheDuration"] = "02:00:00",
            ["Decentralized:ServiceEndpointCacheDuration"] = "02:00:00",
            ["Decentralized:MaxCacheSize"] = "10000",
            ["Decentralized:CacheCleanupInterval"] = "00:30:00",
            ["Decentralized:SupportedDidMethods"] = "web,key",
            ["Decentralized:SupportedVerificationMethodTypes"] = "Ed25519VerificationKey2020,Ed25519VerificationKey2018,JsonWebKey2020,EcdsaSecp256k1VerificationKey2019,EcdsaSecp256k1RecoveryMethod2020,RsaVerificationKey2018",
            ["Decentralized:SupportedServiceTypes"] = "DIDCommMessaging,LinkedDomains,DIDCommV2",
            ["Decentralized:MaxDidLength"] = "2048",
            ["Decentralized:MaxDidDocumentSize"] = "1048576",
            ["Decentralized:MaxVerificationMethods"] = "100",
            ["Decentralized:MaxServiceEndpoints"] = "100",
            ["Decentralized:ResolutionTimeout"] = "00:00:30",
            ["Decentralized:ValidationTimeout"] = "00:00:10",
            ["Decentralized:RetryCount"] = "3",
            ["Decentralized:RetryDelay"] = "00:00:01",
            ["Decentralized:EnableDetailedLogging"] = "false",
            ["Decentralized:EnablePerformanceMetrics"] = "true",
            ["Decentralized:MetricsCollectionInterval"] = "00:05:00",
            ["Decentralized:HealthCheckTimeout"] = "00:00:05",
            ["Decentralized:HealthCheckInterval"] = "00:01:00",
            ["Decentralized:CircuitBreakerFailureThreshold"] = "5",
            ["Decentralized:CircuitBreakerTimeout"] = "00:01:00",
            ["Decentralized:CircuitBreakerRetryTimeout"] = "00:00:30",
            ["Decentralized:RateLimitRequestsPerMinute"] = "1000",
            ["Decentralized:RateLimitBurstSize"] = "100",
            ["Decentralized:RateLimitWindowSize"] = "00:01:00"
        };

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration!)
            .Build();

        // Act
        var result = services.AddDecentralizedAuth(configurationRoot);

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithW3CCompliantAction_ShouldAddServices()
    {
        // Arrange - Action compliant with W3C DID 1.1 spec
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(options =>
        {
            options.EnableDidAuthentication = true;
            options.EnableDidValidation = true;
            options.EnableCaching = true;
            options.DidHeaderName = "X-DID";
            options.DefaultCacheDuration = TimeSpan.FromMinutes(15);
            options.DidDocumentCacheDuration = TimeSpan.FromHours(1);
            options.VerificationMethodCacheDuration = TimeSpan.FromHours(2);
            options.ServiceEndpointCacheDuration = TimeSpan.FromHours(2);
            options.MaxCacheSize = 10000;
            options.CacheCleanupInterval = TimeSpan.FromMinutes(30);
            options.SupportedDidMethods = new List<string> { "web", "key" };
            options.SupportedVerificationMethodTypes = new List<string>
            {
                "Ed25519VerificationKey2020",
                "Ed25519VerificationKey2018",
                "JsonWebKey2020",
                "EcdsaSecp256k1VerificationKey2019",
                "EcdsaSecp256k1RecoveryMethod2020",
                "RsaVerificationKey2018"
            };
            options.SupportedServiceTypes = new List<string>
            {
                "DIDCommMessaging",
                "LinkedDomains",
                "DIDCommV2"
            };
            options.MaxDidLength = 2048;
            options.MaxDidDocumentSize = 1048576;
            options.MaxVerificationMethods = 100;
            options.MaxServiceEndpoints = 100;
            options.ResolutionTimeout = TimeSpan.FromSeconds(30);
            options.ValidationTimeout = TimeSpan.FromSeconds(10);
            options.RetryCount = 3;
            options.RetryDelay = TimeSpan.FromSeconds(1);
            options.EnableDetailedLogging = false;
            options.EnablePerformanceMetrics = true;
            options.MetricsCollectionInterval = TimeSpan.FromMinutes(5);
            options.HealthCheckTimeout = TimeSpan.FromSeconds(5);
            options.HealthCheckInterval = TimeSpan.FromMinutes(1);
            options.CircuitBreakerFailureThreshold = 5;
            options.CircuitBreakerTimeout = TimeSpan.FromMinutes(1);
            options.CircuitBreakerRetryTimeout = TimeSpan.FromSeconds(30);
            options.RateLimitRequestsPerMinute = 1000;
            options.RateLimitBurstSize = 100;
            options.RateLimitWindowSize = TimeSpan.FromMinutes(1);
        });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithW3CCompliantBuilder_ShouldAddServices()
    {
        // Arrange - Builder compliant with W3C DID 1.1 spec
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(builder =>
        {
            builder
                .SetEnabled(true)
                .SetDefaultMethod("web")
                .SetSupportedMethods("web", "key")
                .SetRedisCache("localhost:6379", TimeSpan.FromMinutes(30))
                .SetPostgreSql("Host=localhost;Database=test;Username=user;Password=pass")
                .SetMongoDb("mongodb://localhost:27017", "test_db")
                .SetDefaultCryptoAlgorithm("Ed25519")
                .SetVerifiableCredentials(true, true, TimeSpan.FromDays(365))
                .SetIssuerAndAudience("did:web:issuer.com", "did:web:audience.com");
        });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void AddDecentralizedAuth_WithEmptyConfiguration_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new Dictionary<string, string>();

        var configurationRoot = new ConfigurationBuilder()
            .AddInMemoryCollection(configuration!)
            .Build();

        // Act
        var result = services.AddDecentralizedAuth(configurationRoot);

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithEmptyAction_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(options => { });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithEmptyBuilderAction_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddDecentralizedAuth(builder => { });

        // Assert
        result.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddDecentralizedAuth_WithMultipleCalls_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result1 = services.AddDecentralizedAuth();
        var result2 = services.AddDecentralizedAuth(options => { });

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        services.Should().NotBeEmpty();
    }

    #endregion
}