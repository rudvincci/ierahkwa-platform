using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Authentik;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using System.Diagnostics;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Performance;

[Collection("AuthentikIntegration")]
public class ResponseTimeTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public ResponseTimeTests(AuthentikTestFixture fixture)
    {
        _fixture = fixture;
        
        if (!_fixture.IsContainerRunning || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }
        
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = _fixture.BaseUrl;
            options.ApiToken = _fixture.ApiToken;
        });

        var serviceProvider = services.BuildServiceProvider();
        _client = serviceProvider.GetRequiredService<IAuthentikClient>();
    }

    [Fact]
    public async Task SingleRequest_ShouldCompleteWithinThreshold()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Measure single request latency
        var stopwatch = Stopwatch.StartNew();
        var result = await _client.Core.ListUsersAsync(page: 1, pageSize: 1);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Single request should complete within 5 seconds");
    }

    [Fact]
    public async Task ConcurrentRequests_ShouldHandleLoad()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Test concurrent requests
        const int concurrentRequests = 10;
        var tasks = new List<Task<Mamey.Authentik.Models.PaginatedResult<object>>>();

        var stopwatch = Stopwatch.StartNew();
        
        for (int i = 0; i < concurrentRequests; i++)
        {
            tasks.Add(_client.Core.ListUsersAsync(page: 1, pageSize: 5));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        results.Should().AllSatisfy(r => r.Should().NotBeNull());
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000, "10 concurrent requests should complete within 10 seconds");
    }

    [Fact]
    public async Task MultipleSequentialRequests_ShouldMaintainPerformance()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Test sequential requests to verify no performance degradation
        var times = new List<long>();
        
        for (int i = 0; i < 5; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await _client.Core.ListUsersAsync(page: 1, pageSize: 1);
            stopwatch.Stop();
            result.Should().NotBeNull();
            times.Add(stopwatch.ElapsedMilliseconds);
        }

        // Assert - all requests should complete reasonably fast
        times.Should().AllSatisfy(t => t.Should().BeLessThan(5000));
        
        // Verify consistency - variance should not be too high
        double avg = times.Average();
        long max = times.Max();
        max.Should().BeLessThan((long)(avg * 3), "Response times should be relatively consistent");
    }
}
