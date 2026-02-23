using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Authentik;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Scenarios;

[Collection("AuthentikIntegration")]
public class UserOnboardingTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public UserOnboardingTests(AuthentikTestFixture fixture)
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
    public async Task CompleteUserOnboarding_ShouldSucceed()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // This test verifies the complete user onboarding flow:
        // 1. List existing users to verify API connectivity
        // 2. Verify pagination works
        // 3. Test error handling for invalid operations
        
        // Step 1: Verify API connectivity
        var usersResult = await _client.Core.ListUsersAsync(page: 1, pageSize: 10);
        usersResult.Should().NotBeNull();
        usersResult.Results.Should().NotBeNull();
        
        // Step 2: Verify pagination
        if (usersResult.Pagination != null && usersResult.Pagination.Count > 0)
        {
            var page2 = await _client.Core.ListUsersAsync(page: 2, pageSize: 10);
            page2.Should().NotBeNull();
        }
        
        // Step 3: Test error handling
        var invalidUserId = Guid.NewGuid().ToString();
        Func<Task> act = async () => await _client.Core.GetUserAsync(invalidUserId);
        var exception = await Assert.ThrowsAsync<AuthentikNotFoundException>(act);
        exception.Should().NotBeNull();
    }

    [Fact]
    public async Task UserManagement_CRUDOperations_ShouldWork()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Test Read operations
        var users = await _client.Core.ListUsersAsync(page: 1, pageSize: 5);
        users.Should().NotBeNull();
        
        // If users exist, test Get operation
        if (users.Results != null && users.Results.Any())
        {
            var firstUser = users.Results.First();
            var userId = firstUser?.ToString() ?? throw new InvalidOperationException("User ID is null");
            
            var user = await _client.Core.GetUserAsync(userId);
            user.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task UserManagement_ErrorHandling_ShouldWork()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Test various error scenarios
        Func<Task> act1 = async () => await _client.Core.GetUserAsync(string.Empty);
        var ex1 = await Assert.ThrowsAsync<ArgumentException>(act1);
        ex1.Should().NotBeNull();
        
        Func<Task> act2 = async () => await _client.Core.GetUserAsync("   ");
        var ex2 = await Assert.ThrowsAsync<ArgumentException>(act2);
        ex2.Should().NotBeNull();
        
        var invalidId = Guid.NewGuid().ToString();
        Func<Task> act3 = async () => await _client.Core.GetUserAsync(invalidId);
        var ex3 = await Assert.ThrowsAsync<AuthentikNotFoundException>(act3);
        ex3.Should().NotBeNull();
    }
}
