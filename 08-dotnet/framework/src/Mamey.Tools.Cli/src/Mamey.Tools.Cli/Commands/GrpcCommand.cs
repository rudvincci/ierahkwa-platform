using System.CommandLine;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Blockchain.Node;
using Mamey.Blockchain.Banking;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// gRPC client and testing commands
/// </summary>
public class GrpcCommand : Command
{
    public GrpcCommand(IConfiguration configuration, ILogger logger) : base("grpc", "gRPC client and testing commands")
    {
        var listCommand = new Command("list", "List available services");
        listCommand.SetHandler(() =>
        {
            logger.LogInformation("Available gRPC services:");
            logger.LogInformation("  - BankingService");
            logger.LogInformation("  - GovernmentService");
            logger.LogInformation("  - NodeService");
            logger.LogInformation("  - UniversalProtocolGateway");
        });
        AddCommand(listCommand);
        
        var callCommand = new Command("call", "Call gRPC method");
        var serviceArgument = new Argument<string>("service", "Service name");
        var methodArgument = new Argument<string>("method", "Method name");
        var argsArgument = new Argument<string?>("args", "Method arguments (JSON)");
        
        callCommand.AddArgument(serviceArgument);
        callCommand.AddArgument(methodArgument);
        callCommand.AddArgument(argsArgument);
        
        var institutionIdOption = new Option<string?>(
            aliases: new[] { "--institution-id" },
            description: "Institution ID for credential gating"
        );
        callCommand.AddOption(institutionIdOption);
        
        var credentialsOption = new Option<string?>(
            aliases: new[] { "--credentials" },
            description: "Credential source (env, keychain, file)"
        );
        callCommand.AddOption(credentialsOption);
        
        callCommand.SetHandler(async (service, method, args, institutionId, credentials) =>
        {
            await ExecuteCallAsync(service, method, args, institutionId, credentials, logger);
        }, serviceArgument, methodArgument, argsArgument, institutionIdOption, credentialsOption);
        AddCommand(callCommand);
        
        var smokeCommand = new Command("smoke", "Run minimal staging gRPC smoke tests");
        var endpointOption = new Option<string?>(
            aliases: new[] { "--endpoint", "-e" },
            description: "gRPC endpoint URL (default: http://localhost:50051)"
        );
        smokeCommand.AddOption(endpointOption);
        smokeCommand.SetHandler(async (endpoint) =>
        {
            await ExecuteSmokeAsync(endpoint, logger);
        }, endpointOption);
        AddCommand(smokeCommand);
        
        var testCommand = new Command("test", "Test service endpoints");
        var testServiceArgument = new Argument<string>("service", "Service name");
        testCommand.AddArgument(testServiceArgument);
        testCommand.SetHandler((service) =>
        {
            logger.LogInformation("Testing service: {Service}", service);
            // TODO: Implement service testing
        }, testServiceArgument);
        AddCommand(testCommand);
    }
    
    static async Task ExecuteCallAsync(string service, string method, string? args, string? institutionId, string? credentials, ILogger logger)
    {
        logger.LogInformation("Calling {Service}.{Method}", service, method);
        if (!string.IsNullOrEmpty(args))
        {
            logger.LogInformation("Arguments: {Args}", args);
        }
        if (!string.IsNullOrEmpty(institutionId))
        {
            logger.LogInformation("Institution ID: {InstitutionId}", institutionId);
        }
        if (!string.IsNullOrEmpty(credentials))
        {
            logger.LogInformation("Credentials: {Credentials}", "[REDACTED]");
        }
        // TODO: Implement actual gRPC call
    }
    
    static async Task ExecuteSmokeAsync(string? endpoint, ILogger logger)
    {
        var nodeUrl = endpoint ?? "http://localhost:50051";
        logger.LogInformation("Running minimal staging gRPC smoke tests against {Endpoint}", nodeUrl);
        logger.LogInformation("");
        
        try
        {
            // Initialize Node client
            var nodeOptions = Options.Create(new MameyNodeClientOptions
            {
                NodeUrl = nodeUrl,
                TimeoutSeconds = 30
            });
            using var nodeClient = new MameyNodeClient(nodeOptions, logger);
            
            // Step 1: Node health check
            logger.LogInformation("=== Step 1: Node Health Check ===");
            try
            {
                var nodeInfo = await nodeClient.GetNodeInfoAsync();
                logger.LogInformation("✓ NodeService.GetNodeInfo succeeded");
                logger.LogInformation("  Version: {Version}", nodeInfo.Version);
                logger.LogInformation("  Node ID: {NodeId}", nodeInfo.NodeId);
                logger.LogInformation("  Block Count: {BlockCount}", nodeInfo.BlockCount);
                logger.LogInformation("  Account Count: {AccountCount}", nodeInfo.AccountCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "✗ NodeService.GetNodeInfo failed: {Message}", ex.Message);
                return;
            }
            
            logger.LogInformation("");
            
            // Step 2: RPC facade
            logger.LogInformation("=== Step 2: RPC Facade ===");
            try
            {
                var version = await nodeClient.GetVersionAsync();
                logger.LogInformation("✓ RpcService.Version succeeded");
                logger.LogInformation("  Success: {Success}", version.Success);
                logger.LogInformation("  RPC Version: {RpcVersion}", version.RpcVersion);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "✗ RpcService.Version failed: {Message}", ex.Message);
                return;
            }
            
            logger.LogInformation("");
            
            // Step 3: Banking smoke (optional - requires credentials)
            logger.LogInformation("=== Step 3: Banking Smoke Test (Optional) ===");
            logger.LogInformation("Note: Banking tests require credentials. Skipping for now.");
            logger.LogInformation("To test banking, use: mamey grpc call BankingService CreatePaymentRequest ...");
            
            logger.LogInformation("");
            logger.LogInformation("✓ All smoke tests passed!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Smoke test failed: {Message}", ex.Message);
        }
    }
}
