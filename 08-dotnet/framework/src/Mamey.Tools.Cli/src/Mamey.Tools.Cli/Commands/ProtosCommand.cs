using System.CommandLine;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// Proto file management commands
/// </summary>
public class ProtosCommand : Command
{
    public ProtosCommand(IConfiguration configuration, ILogger logger) : base("protos", "Proto file management commands")
    {
        var syncCommand = new Command("sync", "Sync proto files from MameyNode");
        syncCommand.SetHandler(async () =>
        {
            await ExecuteSyncAsync(logger);
        });
        AddCommand(syncCommand);
        
        var validateCommand = new Command("check", "Validate proto files (calls cargo build -p mamey-rpc)");
        var mameyNodePathOption = new Option<string?>(
            aliases: new[] { "--mameynode-path", "-p" },
            description: "Path to MameyNode directory (default: ../MameyNode or from config)"
        );
        validateCommand.AddOption(mameyNodePathOption);
        validateCommand.SetHandler(async (mameyNodePath) =>
        {
            await ExecuteCheckAsync(mameyNodePath, logger);
        }, mameyNodePathOption);
        AddCommand(validateCommand);
        
        // Alias for backward compatibility
        var validateAlias = new Command("validate", "Alias for 'check'");
        validateAlias.SetHandler(async (mameyNodePath) =>
        {
            await ExecuteCheckAsync(mameyNodePath, logger);
        }, mameyNodePathOption);
        AddCommand(validateAlias);
        
        var generateCommand = new Command("generate", "Generate code from proto files");
        var langOption = new Option<string>(
            aliases: new[] { "--lang", "-l" },
            description: "Target language (rust, dotnet)"
        ) { IsRequired = true };
        generateCommand.AddOption(langOption);
        generateCommand.SetHandler((lang) =>
        {
            logger.LogInformation("Generating {Language} code from proto files...", lang);
            // TODO: Implement code generation
        }, langOption);
        AddCommand(generateCommand);
        
        var diffCommand = new Command("diff", "Show differences between local and remote protos");
        diffCommand.SetHandler(() =>
        {
            logger.LogInformation("Comparing local and remote proto files...");
            // TODO: Implement proto diff
        });
        AddCommand(diffCommand);
    }
    
    static async Task ExecuteSyncAsync(ILogger logger)
    {
        logger.LogInformation("Syncing proto files from MameyNode...");
        // TODO: Implement proto sync
        await Task.CompletedTask;
    }
    
    static async Task ExecuteCheckAsync(string? mameyNodePath, ILogger logger)
    {
        // Determine MameyNode path
        var path = mameyNodePath;
        if (string.IsNullOrEmpty(path))
        {
            // Try to find MameyNode relative to current directory
            var currentDir = Directory.GetCurrentDirectory();
            var possiblePaths = new[]
            {
                Path.Combine(currentDir, "MameyNode"),
                Path.Combine(currentDir, "..", "MameyNode"),
                Path.Combine(currentDir, "..", "..", "MameyNode"),
            };
            
            path = possiblePaths.FirstOrDefault(Directory.Exists);
            
            if (string.IsNullOrEmpty(path))
            {
                logger.LogError("MameyNode directory not found. Please specify --mameynode-path");
                return;
            }
        }
        
        if (!Directory.Exists(path))
        {
            logger.LogError("MameyNode directory does not exist: {Path}", path);
            return;
        }
        
        logger.LogInformation("Validating proto files in {Path}...", path);
        logger.LogInformation("Running: cargo build -p mamey-rpc");
        
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "cargo",
                Arguments = "build -p mamey-rpc",
                WorkingDirectory = path,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            
            using var process = Process.Start(processInfo);
            if (process == null)
            {
                logger.LogError("Failed to start cargo process");
                return;
            }
            
            // Read output asynchronously
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();
            
            await process.WaitForExitAsync();
            
            var output = await outputTask;
            var error = await errorTask;
            
            if (process.ExitCode == 0)
            {
                logger.LogInformation("✓ Proto compilation succeeded");
                if (!string.IsNullOrEmpty(output))
                {
                    logger.LogDebug("Output: {Output}", output);
                }
            }
            else
            {
                logger.LogError("✗ Proto compilation failed with exit code {ExitCode}", process.ExitCode);
                if (!string.IsNullOrEmpty(error))
                {
                    logger.LogError("Error: {Error}", error);
                }
                if (!string.IsNullOrEmpty(output))
                {
                    logger.LogError("Output: {Output}", output);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute cargo build: {Message}", ex.Message);
        }
    }
}
