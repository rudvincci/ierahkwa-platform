using FluentAssertions;
using Mamey.Authentik;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Services;

[Collection("AuthentikIntegration")]
public class SsfServiceIntegrationTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public SsfServiceIntegrationTests(AuthentikTestFixture fixture)
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

        _fixture.BaseUrl.Should().NotBeNullOrEmpty();
        _client.Ssf.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidToken_ThrowsAuthenticationException()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }

        var client = IntegrationTestHelper.CreateClientWithInvalidToken(_fixture.BaseUrl);
        client.Ssf.Should().NotBeNull();
    }

    // TODO: Add actual API method tests after code generation:
    // - GetFederationsAsync (Single Sign-On Federation)
    // - CreateFederationAsync
    // - UpdateFederationAsync
    // - DeleteFederationAsync
    // - etc.
}
