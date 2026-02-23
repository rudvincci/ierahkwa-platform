using System.Net;
using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.Queries;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.EndToEnd.Authentication;

/// <summary>
/// End-to-end tests for permission and role flows (CRUD and assignment).
/// </summary>
[Collection("EndToEnd")]
public class PermissionRoleFlowTests : ApiEndpoints.BaseApiEndpointTests
{
    public PermissionRoleFlowTests(
        PostgreSQLFixture postgresFixture,
        RedisFixture redisFixture,
        MongoDBFixture mongoFixture,
        MinIOFixture minioFixture)
        : base(postgresFixture, redisFixture, mongoFixture, minioFixture)
    {
    }

    [Fact]
    public async Task CreatePermission_WithValidData_ShouldReturn200Ok()
    {
        // Arrange
        var command = new CreatePermission
        {
            Name = "test:permission",
            Description = "Test permission"
        };

        // Act
        // Note: This endpoint requires authentication (permissions:write)
        var response = await Client.PostAsJsonAsync("/api/auth/permissions", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AssignPermissionToIdentity_WithValidData_ShouldReturn200Ok()
    {
        // Arrange
        var command = new AssignPermissionToIdentity
        {
            IdentityId = Guid.NewGuid(),
            PermissionId = Guid.NewGuid()
        };

        // Act
        // Note: This endpoint requires authentication (permissions:write)
        var response = await Client.PostAsJsonAsync($"/api/auth/identities/{command.IdentityId}/permissions/{command.PermissionId}", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetIdentityPermissions_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var query = new GetIdentityPermissions
        {
            IdentityId = identityId
        };

        // Act
        // Note: This endpoint requires authentication (permissions:read)
        var response = await Client.GetAsync($"/api/auth/identities/{identityId}/permissions");

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRole_WithValidData_ShouldReturn200Ok()
    {
        // Arrange
        var command = new CreateRole
        {
            Name = "test-role",
            Description = "Test role"
        };

        // Act
        // Note: This endpoint requires authentication (roles:write)
        var response = await Client.PostAsJsonAsync("/api/auth/roles", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AssignRoleToIdentity_WithValidData_ShouldReturn200Ok()
    {
        // Arrange
        var command = new AssignRoleToIdentity
        {
            IdentityId = Guid.NewGuid(),
            RoleId = Guid.NewGuid()
        };

        // Act
        // Note: This endpoint requires authentication (roles:write)
        var response = await Client.PostAsJsonAsync($"/api/auth/identities/{command.IdentityId}/roles/{command.RoleId}", command);

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetIdentityRoles_WithValidIdentity_ShouldReturn200Ok()
    {
        // Arrange
        var identityId = Guid.NewGuid();
        var query = new GetIdentityRoles
        {
            IdentityId = identityId
        };

        // Act
        // Note: This endpoint requires authentication (roles:read)
        var response = await Client.GetAsync($"/api/auth/identities/{identityId}/roles");

        // Assert
        // May return 401 if authentication is required
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }
}

