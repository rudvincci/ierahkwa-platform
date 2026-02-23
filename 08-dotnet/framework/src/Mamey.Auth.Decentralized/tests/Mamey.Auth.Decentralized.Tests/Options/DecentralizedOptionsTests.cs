using FluentAssertions;
using Mamey.Auth.Decentralized.Options;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Options;

/// <summary>
/// Tests for the DecentralizedOptions class covering W3C DID 1.1 compliance.
/// </summary>
public class DecentralizedOptionsTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithDefaultValues_ShouldCreateOptions()
    {
        // Act
        var options = new DecentralizedOptions();

        // Assert
        options.EnableDidAuthentication.Should().BeTrue();
        options.EnableDidValidation.Should().BeTrue();
        options.EnableCaching.Should().BeTrue();
        options.DidHeaderName.Should().Be("X-DID");
        options.DefaultCacheDuration.Should().Be(TimeSpan.FromMinutes(15));
        options.DidDocumentCacheDuration.Should().Be(TimeSpan.FromHours(1));
        options.VerificationMethodCacheDuration.Should().Be(TimeSpan.FromHours(2));
        options.ServiceEndpointCacheDuration.Should().Be(TimeSpan.FromHours(2));
        options.MaxCacheSize.Should().Be(10000);
        options.CacheCleanupInterval.Should().Be(TimeSpan.FromMinutes(30));
        options.SupportedDidMethods.Should().BeEquivalentTo(new[] { "web", "key" });
        options.SupportedVerificationMethodTypes.Should().BeEquivalentTo(new[]
        {
            "Ed25519VerificationKey2020",
            "RsaVerificationKey2018",
            "Secp256k1VerificationKey2018"
        });
        options.SupportedServiceTypes.Should().BeEquivalentTo(new[]
        {
            "DIDCommMessaging",
            "LinkedDomains",
            "DIDCommV2"
        });
        options.MaxDidLength.Should().Be(2048);
        options.MaxDidDocumentSize.Should().Be(1048576);
        options.MaxVerificationMethods.Should().Be(100);
        options.MaxServiceEndpoints.Should().Be(100);
        options.ResolutionTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.ValidationTimeout.Should().Be(TimeSpan.FromSeconds(10));
        options.RetryCount.Should().Be(3);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
        options.EnableDetailedLogging.Should().BeFalse();
        options.EnablePerformanceMetrics.Should().BeTrue();
        options.MetricsCollectionInterval.Should().Be(TimeSpan.FromMinutes(5));
        options.HealthCheckTimeout.Should().Be(TimeSpan.FromSeconds(5));
        options.HealthCheckInterval.Should().Be(TimeSpan.FromMinutes(1));
        options.CircuitBreakerFailureThreshold.Should().Be(5);
        options.CircuitBreakerTimeout.Should().Be(TimeSpan.FromMinutes(1));
        options.CircuitBreakerRetryTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.RateLimitRequestsPerMinute.Should().Be(1000);
        options.RateLimitBurstSize.Should().Be(100);
        options.RateLimitWindowSize.Should().Be(TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Constructor_WithCustomValues_ShouldCreateOptions()
    {
        // Arrange
        var options = new DecentralizedOptions
        {
            EnableDidAuthentication = false,
            EnableDidValidation = false,
            EnableCaching = false,
            DidHeaderName = "Custom-DID-Header",
            DefaultCacheDuration = TimeSpan.FromMinutes(30),
            DidDocumentCacheDuration = TimeSpan.FromHours(2),
            VerificationMethodCacheDuration = TimeSpan.FromHours(4),
            ServiceEndpointCacheDuration = TimeSpan.FromHours(4),
            MaxCacheSize = 20000,
            CacheCleanupInterval = TimeSpan.FromMinutes(60),
            SupportedDidMethods = new List<string> { "web", "key", "ion" },
            SupportedVerificationMethodTypes = new List<string>
            {
                "Ed25519VerificationKey2020",
                "RsaVerificationKey2018"
            },
            SupportedServiceTypes = new List<string>
            {
                "DIDCommMessaging",
                "LinkedDomains"
            },
            MaxDidLength = 4096,
            MaxDidDocumentSize = 2097152,
            MaxVerificationMethods = 200,
            MaxServiceEndpoints = 200,
            ResolutionTimeout = TimeSpan.FromSeconds(60),
            ValidationTimeout = TimeSpan.FromSeconds(20),
            RetryCount = 5,
            RetryDelay = TimeSpan.FromSeconds(2),
            EnableDetailedLogging = true,
            EnablePerformanceMetrics = false,
            MetricsCollectionInterval = TimeSpan.FromMinutes(10),
            HealthCheckTimeout = TimeSpan.FromSeconds(10),
            HealthCheckInterval = TimeSpan.FromMinutes(2),
            CircuitBreakerFailureThreshold = 10,
            CircuitBreakerTimeout = TimeSpan.FromMinutes(2),
            CircuitBreakerRetryTimeout = TimeSpan.FromMinutes(1),
            RateLimitRequestsPerMinute = 2000,
            RateLimitBurstSize = 200,
            RateLimitWindowSize = TimeSpan.FromMinutes(2)
        };

        // Assert
        options.EnableDidAuthentication.Should().BeFalse();
        options.EnableDidValidation.Should().BeFalse();
        options.EnableCaching.Should().BeFalse();
        options.DidHeaderName.Should().Be("Custom-DID-Header");
        options.DefaultCacheDuration.Should().Be(TimeSpan.FromMinutes(30));
        options.DidDocumentCacheDuration.Should().Be(TimeSpan.FromHours(2));
        options.VerificationMethodCacheDuration.Should().Be(TimeSpan.FromHours(4));
        options.ServiceEndpointCacheDuration.Should().Be(TimeSpan.FromHours(4));
        options.MaxCacheSize.Should().Be(20000);
        options.CacheCleanupInterval.Should().Be(TimeSpan.FromMinutes(60));
        options.SupportedDidMethods.Should().BeEquivalentTo(new[] { "web", "key", "ion" });
        options.SupportedVerificationMethodTypes.Should().BeEquivalentTo(new[]
        {
            "Ed25519VerificationKey2020",
            "RsaVerificationKey2018"
        });
        options.SupportedServiceTypes.Should().BeEquivalentTo(new[]
        {
            "DIDCommMessaging",
            "LinkedDomains"
        });
        options.MaxDidLength.Should().Be(4096);
        options.MaxDidDocumentSize.Should().Be(2097152);
        options.MaxVerificationMethods.Should().Be(200);
        options.MaxServiceEndpoints.Should().Be(200);
        options.ResolutionTimeout.Should().Be(TimeSpan.FromSeconds(60));
        options.ValidationTimeout.Should().Be(TimeSpan.FromSeconds(20));
        options.RetryCount.Should().Be(5);
        options.RetryDelay.Should().Be(TimeSpan.FromSeconds(2));
        options.EnableDetailedLogging.Should().BeTrue();
        options.EnablePerformanceMetrics.Should().BeFalse();
        options.MetricsCollectionInterval.Should().Be(TimeSpan.FromMinutes(10));
        options.HealthCheckTimeout.Should().Be(TimeSpan.FromSeconds(10));
        options.HealthCheckInterval.Should().Be(TimeSpan.FromMinutes(2));
        options.CircuitBreakerFailureThreshold.Should().Be(10);
        options.CircuitBreakerTimeout.Should().Be(TimeSpan.FromMinutes(2));
        options.CircuitBreakerRetryTimeout.Should().Be(TimeSpan.FromMinutes(1));
        options.RateLimitRequestsPerMinute.Should().Be(2000);
        options.RateLimitBurstSize.Should().Be(200);
        options.RateLimitWindowSize.Should().Be(TimeSpan.FromMinutes(2));
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(10001)]
    public void MaxCacheSize_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            MaxCacheSize = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public void MaxVerificationMethods_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            MaxVerificationMethods = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public void MaxServiceEndpoints_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            MaxServiceEndpoints = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(6)]
    public void RetryCount_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            RetryCount = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(101)]
    public void CircuitBreakerFailureThreshold_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            CircuitBreakerFailureThreshold = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(10001)]
    public void RateLimitRequestsPerMinute_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            RateLimitRequestsPerMinute = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1001)]
    public void RateLimitBurstSize_WithInvalidValue_ShouldThrowArgumentOutOfRangeException(int invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            RateLimitBurstSize = invalidValue
        };

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void DidHeaderName_WithInvalidValue_ShouldThrowArgumentException(string invalidValue)
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            DidHeaderName = invalidValue
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedDidMethods_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedDidMethods = null!
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedDidMethods_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedDidMethods = new string[0].ToList()
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedVerificationMethodTypes_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedVerificationMethodTypes = null!
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedVerificationMethodTypes_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedVerificationMethodTypes = new string[0].ToList()
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedServiceTypes_WithNull_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedServiceTypes = null!
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedServiceTypes_WithEmptyArray_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DecentralizedOptions
        {
            SupportedServiceTypes = new string[0].ToList()
        };

        action.Should().Throw<ArgumentException>();
    }

    // Removed old test method - replaced with separate null and empty array tests above

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantValues_ShouldCreateOptions()
    {
        // Arrange - Options compliant with W3C DID 1.1 spec
        var options = new DecentralizedOptions
        {
            SupportedDidMethods = new List<string> { "web", "key" },
            SupportedVerificationMethodTypes = new List<string>
            {
                "Ed25519VerificationKey2020",
                "Ed25519VerificationKey2018",
                "JsonWebKey2020",
                "EcdsaSecp256k1VerificationKey2019",
                "EcdsaSecp256k1RecoveryMethod2020",
                "RsaVerificationKey2018"
            },
            SupportedServiceTypes = new List<string>
            {
                "DIDCommMessaging",
                "LinkedDomains",
                "DIDCommV2"
            },
            MaxDidLength = 2048,
            MaxDidDocumentSize = 1048576,
            ResolutionTimeout = TimeSpan.FromSeconds(30),
            ValidationTimeout = TimeSpan.FromSeconds(10)
        };

        // Assert
        options.SupportedDidMethods.Should().Contain("web");
        options.SupportedDidMethods.Should().Contain("key");
        options.SupportedVerificationMethodTypes.Should().Contain("Ed25519VerificationKey2020");
        options.SupportedVerificationMethodTypes.Should().Contain("Ed25519VerificationKey2018");
        options.SupportedVerificationMethodTypes.Should().Contain("JsonWebKey2020");
        options.SupportedVerificationMethodTypes.Should().Contain("EcdsaSecp256k1VerificationKey2019");
        options.SupportedVerificationMethodTypes.Should().Contain("EcdsaSecp256k1RecoveryMethod2020");
        options.SupportedVerificationMethodTypes.Should().Contain("RsaVerificationKey2018");
        options.SupportedServiceTypes.Should().Contain("DIDCommMessaging");
        options.SupportedServiceTypes.Should().Contain("LinkedDomains");
        options.SupportedServiceTypes.Should().Contain("DIDCommV2");
        options.MaxDidLength.Should().Be(2048);
        options.MaxDidDocumentSize.Should().Be(1048576);
        options.ResolutionTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.ValidationTimeout.Should().Be(TimeSpan.FromSeconds(10));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithMinimalValues_ShouldCreateOptions()
    {
        // Arrange
        var options = new DecentralizedOptions
        {
            EnableDidAuthentication = false,
            EnableDidValidation = false,
            EnableCaching = false,
            MaxCacheSize = 1,
            MaxVerificationMethods = 1,
            MaxServiceEndpoints = 1,
            RetryCount = 0,
            CircuitBreakerFailureThreshold = 1,
            RateLimitRequestsPerMinute = 1,
            RateLimitBurstSize = 1
        };

        // Assert
        options.EnableDidAuthentication.Should().BeFalse();
        options.EnableDidValidation.Should().BeFalse();
        options.EnableCaching.Should().BeFalse();
        options.MaxCacheSize.Should().Be(1);
        options.MaxVerificationMethods.Should().Be(1);
        options.MaxServiceEndpoints.Should().Be(1);
        options.RetryCount.Should().Be(0);
        options.CircuitBreakerFailureThreshold.Should().Be(1);
        options.RateLimitRequestsPerMinute.Should().Be(1);
        options.RateLimitBurstSize.Should().Be(1);
    }

    [Fact]
    public void Constructor_WithMaximumValues_ShouldCreateOptions()
    {
        // Arrange
        var options = new DecentralizedOptions
        {
            MaxCacheSize = 10000,
            MaxVerificationMethods = 100,
            MaxServiceEndpoints = 100,
            RetryCount = 5,
            CircuitBreakerFailureThreshold = 100,
            RateLimitRequestsPerMinute = 10000,
            RateLimitBurstSize = 1000
        };

        // Assert
        options.MaxCacheSize.Should().Be(10000);
        options.MaxVerificationMethods.Should().Be(100);
        options.MaxServiceEndpoints.Should().Be(100);
        options.RetryCount.Should().Be(5);
        options.CircuitBreakerFailureThreshold.Should().Be(100);
        options.RateLimitRequestsPerMinute.Should().Be(10000);
        options.RateLimitBurstSize.Should().Be(1000);
    }

    [Fact]
    public void Constructor_WithNullLists_ShouldCreateOptions()
    {
        // Arrange
        var options = new DecentralizedOptions
        {
            SupportedDidMethods = null,
            SupportedVerificationMethodTypes = null,
            SupportedServiceTypes = null
        };

        // Assert
        options.SupportedDidMethods.Should().BeNull();
        options.SupportedVerificationMethodTypes.Should().BeNull();
        options.SupportedServiceTypes.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmptyLists_ShouldCreateOptions()
    {
        // Arrange
        var options = new DecentralizedOptions
        {
            SupportedDidMethods = new List<string>(),
            SupportedVerificationMethodTypes = new List<string>(),
            SupportedServiceTypes = new List<string>()
        };

        // Assert
        options.SupportedDidMethods.Should().BeEmpty();
        options.SupportedVerificationMethodTypes.Should().BeEmpty();
        options.SupportedServiceTypes.Should().BeEmpty();
    }

    #endregion
}
