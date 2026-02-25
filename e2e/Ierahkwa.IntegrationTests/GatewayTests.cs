using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Ierahkwa.IntegrationTests;

public class GatewayTests
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public GatewayTests()
    {
        _baseUrl = Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "http://localhost:5000";
        _client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("status").GetString().Should().Be("healthy");
        body.GetProperty("platform").GetString().Should().Be("Ierahkwa Sovereign Platform");
        body.GetProperty("version").GetString().Should().Be("3.0.0");
        body.GetProperty("microservices").GetInt32().Should().Be(83);
    }

    [Fact]
    public async Task Services_ReturnsServiceCatalog()
    {
        var response = await _client.GetAsync("/services");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("gateway").GetString().Should().Be("https://api.ierahkwa.org");
    }

    [Fact]
    public async Task Auth_Login_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new
        {
            userId = "test-user-001",
            tenantId = "navajo",
            tier = "citizen",
            roles = new[] { "user", "admin" }
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("expiresIn").GetInt32().Should().Be(86400);
        body.GetProperty("tier").GetString().Should().Be("citizen");
    }

    [Fact]
    public async Task Auth_Me_Unauthorized_Without_Token()
    {
        var response = await _client.GetAsync("/auth/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Auth_Me_ReturnsUserInfo_WithToken()
    {
        // Login first
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            userId = "test-user-002",
            tenantId = "cherokee",
            tier = "resident",
            roles = new[] { "user" }
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginBody.GetProperty("token").GetString();

        // Call /auth/me with token
        var request = new HttpRequestMessage(HttpMethod.Get, "/auth/me");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("userId").GetString().Should().Be("test-user-002");
        body.GetProperty("tenantId").GetString().Should().Be("cherokee");
        body.GetProperty("tier").GetString().Should().Be("resident");
    }

    [Fact]
    public async Task Auth_Refresh_Works_WithValidToken()
    {
        var loginResponse = await _client.PostAsJsonAsync("/auth/login", new
        {
            userId = "test-refresh",
            tenantId = "lakota",
            tier = "member",
            roles = new[] { "user" }
        });
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        var token = loginBody.GetProperty("token").GetString();

        var refreshResponse = await _client.PostAsJsonAsync("/auth/refresh", new { token });
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshBody = await refreshResponse.Content.ReadFromJsonAsync<JsonElement>();
        refreshBody.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        refreshBody.GetProperty("token").GetString().Should().NotBe(token);
    }

    [Fact]
    public async Task Auth_Refresh_Fails_WithInvalidToken()
    {
        var response = await _client.PostAsJsonAsync("/auth/refresh", new { token = "invalid.token.here" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Api_Routes_Require_Auth()
    {
        // All /api/* routes should require authentication
        var routes = new[] { "/api/space", "/api/health", "/api/governance", "/api/commerce" };
        foreach (var route in routes)
        {
            var response = await _client.GetAsync(route);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Forbidden,
                HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable);
        }
    }
}
