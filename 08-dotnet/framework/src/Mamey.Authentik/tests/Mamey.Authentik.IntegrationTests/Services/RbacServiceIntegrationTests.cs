using FluentAssertions;
using Mamey.Authentik;
using Mamey.Authentik.IntegrationTests.TestFixtures;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.Services;

[Collection("AuthentikIntegration")]
public class RbacServiceIntegrationTests : IClassFixture<AuthentikTestFixture>
{
    private readonly AuthentikTestFixture _fixture;
    private readonly IAuthentikClient? _client;

    public RbacServiceIntegrationTests(AuthentikTestFixture fixture)
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
        _client.Rbac.Should().NotBeNull();
    }

    [Fact]
    public async Task Service_WithInvalidToken_ThrowsAuthenticationException()
    {
        if (!_fixture.IsContainerRunning)
        {
            return;
        }

        var client = IntegrationTestHelper.CreateClientWithInvalidToken(_fixture.BaseUrl);
        client.Rbac.Should().NotBeNull();
    }

    // TODO: Add actual API method tests after code generation:
    // - GetRolesAsync
    // - CreateRoleAsync
    // - UpdateRoleAsync
    // - DeleteRoleAsync
    // - GetPermissionsAsync
    // - AssignPermissionAsync
    // - etc.
}
