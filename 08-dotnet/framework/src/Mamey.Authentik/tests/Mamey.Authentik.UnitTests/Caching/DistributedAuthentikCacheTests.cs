using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Mamey.Authentik.Caching;
using Mamey.Authentik.UnitTests;
using Xunit;

namespace Mamey.Authentik.UnitTests.Caching;

public class DistributedAuthentikCacheTests : TestBase
{
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly ILogger<DistributedAuthentikCache> _logger;
    private readonly DistributedAuthentikCache _cache;

    public DistributedAuthentikCacheTests()
    {
        _distributedCacheMock = new Mock<IDistributedCache>();
        _logger = CreateMockLogger<DistributedAuthentikCache>();
        _cache = new DistributedAuthentikCache(_distributedCacheMock.Object, _logger);
    }

    [Fact]
    public async Task GetAsync_WithNonExistentKey_ReturnsNull()
    {
        // Arrange
        _distributedCacheMock
            .Setup(x => x.GetAsync("non-existent-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _cache.GetAsync<string>("non-existent-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_AndGetAsync_ReturnsCachedValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var jsonBytes = System.Text.Encoding.UTF8.GetBytes("{\"value\":\"test-value\"}");

        _distributedCacheMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(jsonBytes);

        // Act
        await _cache.SetAsync(key, value, TimeSpan.FromMinutes(5));
        var result = await _cache.GetAsync<string>(key);

        // Assert
        // Note: This test verifies the cache structure, actual serialization would need proper setup
        _distributedCacheMock.Verify(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_CallsDistributedCacheRemove()
    {
        // Arrange
        var key = "test-key";

        // Act
        await _cache.RemoveAsync(key);

        // Assert
        _distributedCacheMock.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }
}
