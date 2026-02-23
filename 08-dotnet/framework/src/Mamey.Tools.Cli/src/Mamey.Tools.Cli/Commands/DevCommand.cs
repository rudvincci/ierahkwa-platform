using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// Start development node with hot-reload
/// </summary>
public class DevCommand : Command
{
    public DevCommand(IConfiguration configuration, ILogger logger) : base("dev", "Start development node with hot-reload")
    {
        var portOption = new Option<int?>(
            aliases: new[] { "--port", "-p" },
            description: "gRPC port (default: 50051)"
        );
        
        AddOption(portOption);
        
        var networkOption = new Option<string?>(
            aliases: new[] { "--network", "-n" },
            description: "Network configuration (local, testnet, mainnet)"
        );
        
        AddOption(networkOption);
        
        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Verbose logging"
        );
        
        AddOption(verboseOption);
        
        var noReloadOption = new Option<bool>(
            aliases: new[] { "--no-reload" },
            description: "Disable hot-reload"
        );
        
        AddOption(noReloadOption);
        
        this.SetHandler(async (port, network, verbose, noReload) =>
        {
            await ExecuteAsync(port, network, verbose, noReload, logger);
        }, portOption, networkOption, verboseOption, noReloadOption);
    }
    
    static async Task ExecuteAsync(int? port, string? network, bool verbose, bool noReload, ILogger logger)
    {
        logger.LogInformation("Starting development node...");
        logger.LogInformation("Port: {Port}", port ?? 50051);
        logger.LogInformation("Network: {Network}", network ?? "local");
        logger.LogInformation("Hot-reload: {HotReload}", !noReload);
        
        // TODO: Implement actual node startup
        logger.LogWarning("Node startup not yet implemented");
        logger.LogInformation("Development node would start here");
        
        // Keep running until interrupted
        await Task.Delay(Timeout.Infinite);
    }
}
