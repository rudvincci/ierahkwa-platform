using System.Net;
using System.Net.Http.Json;
using Pupitre.Tests.Shared.Fixtures;
using Pupitre.Users.Contracts.Commands;
using Xunit;

namespace Pupitre.Tests.Integration.ApiTests;

[Collection("Integration")]
public class UsersApiTests : IClassFixture<ApiGatewayClientFixture>
{
    private readonly ApiGatewayClientFixture _clientFixture;

    public UsersApiTests(ApiGatewayClientFixture clientFixture)
    {
        _clientFixture = clientFixture;
    }

    [Fact(Skip = "Requires running API Gateway and Users service")]
    public async Task Post_Then_Get_ShouldSucceed()
    {
        // Arrange
        var command = new AddUser(
            id: Guid.NewGuid(),
            name: "Integration User",
            tags: new[] { "integration" },
            citizenId: $"INT-{Guid.NewGuid():N}".Substring(0, 12),
            firstName: "Integration",
            lastName: "Tester",
            nationality: "USA",
            programCode: "INT-TEST",
            credentialType: "Profile");

        // Act
        var createResponse = await _clientFixture.Client.PostAsJsonAsync("/api/users", command);

        // Assert
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var getResponse = await _clientFixture.Client.GetAsync($"/api/users/{command.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
}
