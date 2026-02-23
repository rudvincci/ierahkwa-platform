using FluentAssertions;
using Mamey.Authentik;
using Mamey.Authentik.Exceptions;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using System;
using System.Net;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Scenarios;

[Collection("AuthentikIntegration")]
public class ErrorHandlingTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public ErrorHandlingTests(AuthentikTestFixture fixture)
    {
        _fixture = fixture;
        _client = IntegrationTestHelper.CreateClient(_fixture);
    }

    [Fact]
    public async Task InvalidAuthentication_ThrowsAuthenticationException()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }

        // Arrange
        var client = IntegrationTestHelper.CreateClientWithInvalidToken(_fixture.BaseUrl);

        // Act & Assert
        Func<Task> act = async () => await client.Core.ListUsersAsync();
        var exception = await Assert.ThrowsAsync<AuthentikAuthenticationException>(act);
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task NotFoundResource_ThrowsNotFoundException()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Arrange
        var invalidUserId = Guid.NewGuid().ToString();

        // Act & Assert
        Func<Task> act = async () => await _client.Core.GetUserAsync(invalidUserId);
        var exception = await Assert.ThrowsAsync<AuthentikNotFoundException>(act);
        
        exception.Should().NotBeNull();
        exception.StatusCode.Should().Be(404);
        exception.ResourceId.Should().Be(invalidUserId);
    }

    [Fact]
    public async Task InvalidInput_ThrowsArgumentException()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Act & Assert
        Func<Task> act1 = async () => await _client.Core.GetUserAsync(string.Empty);
        var ex1 = await Assert.ThrowsAsync<ArgumentException>(act1);
        ex1.Should().NotBeNull();

        Func<Task> act2 = async () => await _client.Core.GetUserAsync("   ");
        var ex2 = await Assert.ThrowsAsync<ArgumentException>(act2);
        ex2.Should().NotBeNull();
    }

    [Fact]
    public async Task NetworkError_ThrowsException()
    {
        // Arrange
        var client = IntegrationTestHelper.CreateClientWithInvalidUrl();

        // Act & Assert
        Func<Task> act = async () => await client.Core.ListUsersAsync();
        var exception = await Assert.ThrowsAnyAsync<Exception>(act);
        
        exception.Should().NotBeNull();
        // Could be HttpRequestException, TaskCanceledException, etc.
    }

    [Fact]
    public async Task Exception_ContainsRequestInformation()
    {
        if (!_fixture.IsContainerRunning || _client == null || string.IsNullOrWhiteSpace(_fixture.ApiToken))
        {
            return;
        }

        // Arrange
        var invalidUserId = Guid.NewGuid().ToString();

        // Act
        Func<Task> act = async () => await _client.Core.GetUserAsync(invalidUserId);
        var exception = await Assert.ThrowsAsync<AuthentikNotFoundException>(act);

        // Assert
        exception.Should().NotBeNull();
        exception.RequestUri.Should().NotBeNullOrEmpty();
        exception.RequestUri.Should().Contain(invalidUserId);
    }
}
