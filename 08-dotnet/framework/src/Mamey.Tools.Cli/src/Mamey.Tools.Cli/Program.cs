using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Tools.Cli.Commands;
using Mamey.Tools.Cli.Configuration;

namespace Mamey.Tools.Cli;

/// <summary>
/// MameyNode CLI - Hardhat-style developer tool
/// </summary>
class Program
{
    static async Task<int> Main(string[] args)
    {
        // Load configuration
        var configuration = LoadConfiguration();
        
        // Setup logging
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder =>
        {
            builder.AddConsole();
            var logLevel = configuration.GetValue<string>("Logging:Level") ?? "Info";
            if (Enum.TryParse<LogLevel>(logLevel, ignoreCase: true, out var level))
            {
                builder.SetMinimumLevel(level);
            }
        });
        
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddSingleton<MameyConfig>(sp =>
        {
            var config = new MameyConfig();
            configuration.Bind(config);
            return config;
        });
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        // Create root command
        var rootCommand = new RootCommand("MameyNode CLI - Hardhat-style developer tool for MameyNode blockchain")
        {
            Name = "mamey"
        };
        
        // Add subcommands
        rootCommand.AddCommand(new InitCommand(configuration, logger));
        rootCommand.AddCommand(new DevCommand(configuration, logger));
        rootCommand.AddCommand(new TestCommand(configuration, logger));
        rootCommand.AddCommand(new WorkflowsCommand(configuration, logger));
        rootCommand.AddCommand(new GrpcCommand(configuration, logger));
        rootCommand.AddCommand(new ProtosCommand(configuration, logger));
        rootCommand.AddCommand(new NodeCommand(configuration, logger));
        rootCommand.AddCommand(new CredentialsCommand(configuration, logger));
        rootCommand.AddCommand(new ConfigCommand(configuration, logger));
        
        // Set global options
        rootCommand.AddGlobalOption(new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose logging"
        ));
        
        // Execute command
        try
        {
            return await rootCommand.InvokeAsync(args);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
    
    static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("mamey.config.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("MAMEY_")
            .AddEnvironmentVariables();
        
        return builder.Build();
    }
}
