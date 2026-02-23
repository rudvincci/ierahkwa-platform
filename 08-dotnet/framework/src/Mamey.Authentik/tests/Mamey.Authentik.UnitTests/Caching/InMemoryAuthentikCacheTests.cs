using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mamey.Authentik.Caching;
using Mamey.Authentik.UnitTests;
using Xunit;

namespace Mamey.Authentik.UnitTests.Caching;

public class InMemoryAuthentikCacheTests : TestBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryAuthentikCache> _logger;
    private readonly InMemoryAuthentikCache _cache;

    public InMemoryAuthentikCacheTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = CreateMockLogger<InMemoryAuthentikCache>();
        _cache = new InMemoryAuthentikCache(_memoryCache, _logger);
    }

    [Fact]
    public async Task GetAsync_WithNonExistentKey_ReturnsNull()
    {
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
        var ttl = TimeSpan.FromMinutes(5);

        // Act
        await _cache.SetAsync(key, value, ttl);
        var result = await _cache.GetAsync<string>(key);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task SetAsync_WithExpiredTtl_ReturnsNull()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var ttl = TimeSpan.FromMilliseconds(100);

        // Act
        await _cache.SetAsync(key, value, ttl);
        await Task.Delay(150); // Wait for expiration
        var result = await _cache.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveAsync_RemovesCachedValue()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        await _cache.SetAsync(key, value, TimeSpan.FromMinutes(5));

        // Act
        await _cache.RemoveAsync(key);
        var result = await _cache.GetAsync<string>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveByPatternAsync_RemovesMatchingKeys()
    {
        // Arrange
        await _cache.SetAsync("user:1", "user1", TimeSpan.FromMinutes(5));
        await _cache.SetAsync("user:2", "user2", TimeSpan.FromMinutes(5));
        await _cache.SetAsync("group:1", "group1", TimeSpan.FromMinutes(5));

        // Act
        await _cache.RemoveByPatternAsync("user:");
        
        // Assert
        (await _cache.GetAsync<string>("user:1")).Should().BeNull();
        (await _cache.GetAsync<string>("user:2")).Should().BeNull();
        (await _cache.GetAsync<string>("group:1")).Should().Be("group1");
    }

    [Fact]
    public async Task ClearAsync_RemovesAllCachedValues()
    {
        // Arrange
        await _cache.SetAsync("key1", "value1", TimeSpan.FromMinutes(5));
        await _cache.SetAsync("key2", "value2", TimeSpan.FromMinutes(5));

        // Act
        await _cache.ClearAsync();
        
        // Assert
        (await _cache.GetAsync<string>("key1")).Should().BeNull();
        (await _cache.GetAsync<string>("key2")).Should().BeNull();
    }
}
