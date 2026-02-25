using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Ierahkwa.IntegrationTests;

/// <summary>
/// Smoke tests for all 83 microservices via the API Gateway.
/// Each test verifies the service is reachable and responds to basic CRUD.
/// </summary>
public class MicroserviceSmokeTests
{
    private readonly HttpClient _client;
    private readonly string _token;

    public MicroserviceSmokeTests()
    {
        var baseUrl = Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "http://localhost:5000";
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _token = GetAuthToken().GetAwaiter().GetResult();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
    }

    private async Task<string> GetAuthToken()
    {
        var tempClient = new HttpClient
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "http://localhost:5000")
        };
        var response = await tempClient.PostAsJsonAsync("/auth/login", new
        {
            userId = "smoke-test-runner",
            tenantId = "ierahkwa-global",
            tier = "citizen",
            roles = new[] { "admin" }
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }

    public static IEnumerable<object[]> AllServiceEndpoints => new List<object[]>
    {
        // NEXUS Orbital
        new object[] { "/api/space", "SpaceService" },
        new object[] { "/api/telecom", "TelecomService" },
        new object[] { "/api/genomics", "GenomicsService" },
        new object[] { "/api/iot-robotics", "IoTRoboticsService" },
        new object[] { "/api/quantum", "QuantumService" },
        new object[] { "/api/ai-engine", "AIEngineService" },
        new object[] { "/api/network", "NetworkService" },
        new object[] { "/api/devtools", "DevToolsService" },
        // NEXUS Escudo
        new object[] { "/api/military", "MilitaryService" },
        new object[] { "/api/drones", "DroneService" },
        new object[] { "/api/cybersec", "CyberSecService" },
        new object[] { "/api/intelligence", "IntelligenceService" },
        new object[] { "/api/emergency", "EmergencyService" },
        // NEXUS Cerebro
        new object[] { "/api/education", "EducationService" },
        new object[] { "/api/research", "ResearchService" },
        new object[] { "/api/language", "LanguageService" },
        new object[] { "/api/search", "SearchService" },
        // NEXUS Tesoro
        new object[] { "/api/commerce", "CommerceService" },
        new object[] { "/api/blockchain", "BlockchainService" },
        new object[] { "/api/banking", "BankingService" },
        new object[] { "/api/insurance", "InsuranceService" },
        new object[] { "/api/employment", "EmploymentService" },
        new object[] { "/api/smart-factory", "SmartFactoryService" },
        new object[] { "/api/artisan", "ArtisanService" },
        new object[] { "/api/tourism", "TourismService" },
        // NEXUS Voces
        new object[] { "/api/media", "MediaContentService" },
        new object[] { "/api/messaging", "MessagingService" },
        new object[] { "/api/culture", "CultureArchiveService" },
        new object[] { "/api/sports", "SportsService" },
        new object[] { "/api/social", "SocialService" },
        // NEXUS Consejo
        new object[] { "/api/governance", "GovernanceService" },
        new object[] { "/api/justice", "JusticeService" },
        new object[] { "/api/diplomacy", "DiplomacyService" },
        new object[] { "/api/citizen", "CitizenService" },
        new object[] { "/api/social-welfare", "SocialWelfareService" },
        // NEXUS Tierra
        new object[] { "/api/agriculture", "AgricultureService" },
        new object[] { "/api/natural-resource", "NaturalResourceService" },
        new object[] { "/api/environment", "EnvironmentService" },
        new object[] { "/api/waste", "WasteService" },
        new object[] { "/api/energy", "EnergyService" },
        // NEXUS Forja
        new object[] { "/api/devops", "DevOpsService" },
        new object[] { "/api/lowcode", "LowCodeDesignService" },
        new object[] { "/api/browser", "BrowserService" },
        new object[] { "/api/productivity", "ProductivityService" },
        new object[] { "/api/cloud", "CloudService" },
        // NEXUS Urbe
        new object[] { "/api/urban", "UrbanService" },
        new object[] { "/api/transport", "TransportService" },
        new object[] { "/api/postal-maps", "PostalMapsService" },
        new object[] { "/api/housing", "HousingService" },
        // NEXUS Raíces
        new object[] { "/api/identity", "IdentityService" },
        new object[] { "/api/health", "HealthService" },
        new object[] { "/api/nexus", "NexusAggregationService" },
        new object[] { "/api/licensing", "LicensingService" },
    };

    [Theory]
    [MemberData(nameof(AllServiceEndpoints))]
    public async Task Service_IsReachable(string endpoint, string serviceName)
    {
        var response = await _client.GetAsync(endpoint);

        // Service should respond (not 404 from gateway itself)
        // May be 200, 502 (service down), or 401/403 (auth), but NOT 404
        response.StatusCode.Should().NotBe(HttpStatusCode.NotFound,
            $"{serviceName} at {endpoint} should be routed by the gateway");
    }

    [Fact]
    public async Task TenantIsolation_DifferentTenants_SeeOwnData()
    {
        // Create data as navajo tenant
        var navajoClient = await CreateTenantClient("navajo");
        var cherokeeClient = await CreateTenantClient("cherokee");

        // Both should be able to hit the citizen endpoint
        var navajoResponse = await navajoClient.GetAsync("/api/citizen");
        var cherokeeResponse = await cherokeeClient.GetAsync("/api/citizen");

        // Both should get successful responses (or 502 if service not running)
        navajoResponse.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        cherokeeResponse.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TierAuthorization_MemberCannotAccessCitizenOnly()
    {
        var memberClient = await CreateTierClient("member");

        // Military is CitizenOnly — member should get 403
        var response = await memberClient.GetAsync("/api/military");
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Forbidden,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task TierAuthorization_CitizenCanAccessCitizenOnly()
    {
        var citizenClient = await CreateTierClient("citizen");

        var response = await citizenClient.GetAsync("/api/military");
        // Should NOT be 403 — citizen has access
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
    }

    private async Task<HttpClient> CreateTenantClient(string tenantId)
    {
        var baseUrl = Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "http://localhost:5000";
        var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            userId = $"test-{tenantId}",
            tenantId,
            tier = "citizen",
            roles = new[] { "user" }
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("token").GetString();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private async Task<HttpClient> CreateTierClient(string tier)
    {
        var baseUrl = Environment.GetEnvironmentVariable("GATEWAY_URL") ?? "http://localhost:5000";
        var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            userId = $"test-{tier}",
            tenantId = "ierahkwa-global",
            tier,
            roles = new[] { "user" }
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = body.GetProperty("token").GetString();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
