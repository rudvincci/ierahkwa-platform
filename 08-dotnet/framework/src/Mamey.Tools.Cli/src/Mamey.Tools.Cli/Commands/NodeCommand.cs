using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// Node management commands
/// </summary>
public class NodeCommand : Command
{
    public NodeCommand(IConfiguration configuration, ILogger logger) : base("node", "Node management commands")
    {
        var startCommand = new Command("start", "Start node");
        startCommand.SetHandler(() =>
        {
            logger.LogInformation("Starting node...");
            // TODO: Implement node start
        });
        AddCommand(startCommand);
        
        var stopCommand = new Command("stop", "Stop node");
        stopCommand.SetHandler(() =>
        {
            logger.LogInformation("Stopping node...");
            // TODO: Implement node stop
        });
        AddCommand(stopCommand);
        
        var statusCommand = new Command("status", "Get node status");
        statusCommand.SetHandler(() =>
        {
            logger.LogInformation("Getting node status...");
            // TODO: Implement node status
        });
        AddCommand(statusCommand);
        
        var logsCommand = new Command("logs", "View node logs");
        logsCommand.SetHandler(() =>
        {
            logger.LogInformation("Viewing node logs...");
            // TODO: Implement node logs
        });
        AddCommand(logsCommand);
    }
}
