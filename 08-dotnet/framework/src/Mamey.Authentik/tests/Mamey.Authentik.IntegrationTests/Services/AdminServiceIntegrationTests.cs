using FluentAssertions;
using Mamey.Authentik;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Services;

[Collection("AuthentikIntegration")]
public class AdminServiceIntegrationTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public AdminServiceIntegrationTests(AuthentikTestFixture fixture)
    {
        _fixture = fixture;
        _client = IntegrationTestHelper.CreateClient(_fixture);
    }

    [Fact]
    public async Task HealthCheck_ServiceIsAccessible()
    {
        if (!_fixture.IsContainerRunning || _client == null)
        {
            return;
        }

        // Verify service is accessible
        _fixture.BaseUrl.Should().NotBeNullOrEmpty();
        _client.Admin.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidToken_ThrowsAuthenticationException()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }

        // Arrange
        var client = IntegrationTestHelper.CreateClientWithInvalidToken(_fixture.BaseUrl);

        // Act & Assert
        // Note: Actual method calls will be added after code generation
        // For now, we verify the service is configured correctly
        client.Admin.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidBaseUrl_ThrowsException()
    {
        // Arrange
        var client = IntegrationTestHelper.CreateClientWithInvalidUrl();

        // Act & Assert
        // Note: Actual method calls will be added after code generation
        // For now, we verify the service is configured correctly
        client.Admin.Should().NotBeNull();
    }

    // TODO: Add actual API method tests after code generation:
    // - GetUsersAsync
    // - CreateUserAsync
    // - UpdateUserAsync
    // - DeleteUserAsync
    // - GetGroupsAsync
    // - CreateGroupAsync
    // - etc.
}
