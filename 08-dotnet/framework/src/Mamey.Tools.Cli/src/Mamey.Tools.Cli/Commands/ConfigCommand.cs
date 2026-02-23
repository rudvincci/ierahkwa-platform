using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// Configuration management commands
/// </summary>
public class ConfigCommand : Command
{
    public ConfigCommand(IConfiguration configuration, ILogger logger) : base("config", "Configuration management commands")
    {
        var showCommand = new Command("show", "Show current configuration");
        showCommand.SetHandler(() =>
        {
            logger.LogInformation("Current configuration:");
            // TODO: Display configuration
        });
        AddCommand(showCommand);
        
        var setCommand = new Command("set", "Set configuration value");
        var keyArgument = new Argument<string>("key", "Configuration key");
        var valueArgument = new Argument<string>("value", "Configuration value");
        setCommand.AddArgument(keyArgument);
        setCommand.AddArgument(valueArgument);
        setCommand.SetHandler((key, value) =>
        {
            logger.LogInformation("Setting {Key} = {Value}", key, value);
            // TODO: Implement config set
        }, keyArgument, valueArgument);
        AddCommand(setCommand);
        
        var getCommand = new Command("get", "Get configuration value");
        var getKeyArgument = new Argument<string>("key", "Configuration key");
        getCommand.AddArgument(getKeyArgument);
        getCommand.SetHandler((key) =>
        {
            logger.LogInformation("Getting {Key}", key);
            // TODO: Implement config get
        }, getKeyArgument);
        AddCommand(getCommand);
    }
}
