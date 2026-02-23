using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Blockchain.Node;
using Mamey.Node;
using Mamey.Rpc;
using Xunit;

namespace Mamey.Blockchain.Node.Tests;

/// <summary>
/// Smoke tests for minimal staging gRPC surface
/// These tests verify that the .NET SDK can call the minimal staging endpoints
/// as defined in GRPC_SURFACE_MINIMAL_STAGING.json
/// </summary>
public class SmokeTest : IClassFixture<SmokeTestFixture>
{
    private readonly MameyNodeClient _client;
    private readonly SmokeTestFixture _fixture;

    public SmokeTest(SmokeTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    [Fact]
    [Trait("Category", "Smoke")]
    [Trait("Priority", "Critical")]
    public async Task GetNodeInfo_ReturnsValidResponse()
    {
        // Arrange
        var request = new GetNodeInfoRequest();

        // Act
        var response = await _client.GetNodeInfoAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.False(string.IsNullOrEmpty(response.Version), "Version should be non-empty");
        Assert.False(string.IsNullOrEmpty(response.NodeId), "NodeId should be non-empty");
        Assert.True(response.BlockCount >= 0, "BlockCount should be >= 0");
        Assert.True(response.AccountCount >= 0, "AccountCount should be >= 0");
    }

    [Fact]
    [Trait("Category", "Smoke")]
    [Trait("Priority", "Critical")]
    public async Task GetVersion_ReturnsValidResponse()
    {
        // Arrange
        var request = new VersionRequest();

        // Act
        var response = await _client.GetVersionAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success, "Success should be true");
        Assert.True(response.RpcVersion > 0, "RpcVersion should be present");
    }

    [Fact]
    [Trait("Category", "Smoke")]
    [Trait("Priority", "Critical")]
    public async Task GetNodeInfo_WithMetadata_InjectsCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var request = new GetNodeInfoRequest();

        // Act
        var response = await _client.GetNodeInfoAsync(
            request,
            correlationId: correlationId
        );

        // Assert
        Assert.NotNull(response);
        // Note: Correlation ID is in metadata, not response
        // This test verifies the call succeeds with metadata injection
    }

    [Fact]
    [Trait("Category", "Smoke")]
    [Trait("Priority", "Critical")]
    public async Task GetVersion_WithMetadata_InjectsCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var request = new VersionRequest();

        // Act
        var response = await _client.GetVersionAsync(
            request,
            correlationId: correlationId
        );

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact]
    [Trait("Category", "Smoke")]
    [Trait("Priority", "Critical")]
    public async Task SmokeSequence_NodeHealth_Then_RpcFacade()
    {
        // This test mirrors the smoke sequence from GRPC_SURFACE_MINIMAL_STAGING.json
        
        // Step 1: Node health
        var nodeInfo = await _client.GetNodeInfoAsync();
        Assert.False(string.IsNullOrEmpty(nodeInfo.NodeId));
        Assert.False(string.IsNullOrEmpty(nodeInfo.Version));

        // Step 2: RPC facade
        var version = await _client.GetVersionAsync();
        Assert.True(version.Success);
        Assert.True(version.RpcVersion > 0);
    }
}

/// <summary>
/// Test fixture for smoke tests
/// </summary>
public class SmokeTestFixture : IDisposable
{
    public MameyNodeClient Client { get; }

    public SmokeTestFixture()
    {
        var nodeUrl = Environment.GetEnvironmentVariable("MAMEYNODE_ENDPOINT") 
            ?? "http://localhost:50051";
        
        var options = Options.Create(new MameyNodeClientOptions
        {
            NodeUrl = nodeUrl,
            TimeoutSeconds = 30
        });

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<MameyNodeClient>();

        Client = new MameyNodeClient(options, logger);
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}
