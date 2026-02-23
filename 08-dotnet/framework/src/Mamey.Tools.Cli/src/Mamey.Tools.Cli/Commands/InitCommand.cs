using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.Tools.Cli.Commands;

/// <summary>
/// Initialize a new MameyNode project
/// </summary>
public class InitCommand : Command
{
    public InitCommand(IConfiguration configuration, ILogger logger) : base("init", "Initialize a new MameyNode project")
    {
        var projectNameArgument = new Argument<string?>(
            name: "project-name",
            description: "Project name (optional, defaults to current directory)"
        );
        
        AddArgument(projectNameArgument);
        
        var templateOption = new Option<string?>(
            aliases: new[] { "--template", "-t" },
            description: "Project template (banking, government, basic)"
        );
        
        AddOption(templateOption);
        
        var skipInstallOption = new Option<bool>(
            aliases: new[] { "--skip-install" },
            description: "Skip dependency installation"
        );
        
        AddOption(skipInstallOption);
        
        this.SetHandler(async (projectName, template, skipInstall) =>
        {
            await ExecuteAsync(projectName, template, skipInstall, logger);
        }, projectNameArgument, templateOption, skipInstallOption);
    }
    
    static async Task ExecuteAsync(string? projectName, string? template, bool skipInstall, ILogger logger)
    {
        logger.LogInformation("Initializing MameyNode project...");
        
        var projectDir = projectName != null 
            ? Path.Combine(Directory.GetCurrentDirectory(), projectName)
            : Directory.GetCurrentDirectory();
        
        if (!Directory.Exists(projectDir))
        {
            Directory.CreateDirectory(projectDir);
        }
        
        // Create mamey.config.json
        var configPath = Path.Combine(projectDir, "mamey.config.json");
        var configContent = GenerateConfigJson(template ?? "basic");
        await File.WriteAllTextAsync(configPath, configContent);
        logger.LogInformation("Created mamey.config.json");
        
        // Create .mamey directory
        var mameyDir = Path.Combine(projectDir, ".mamey");
        Directory.CreateDirectory(mameyDir);
        logger.LogInformation("Created .mamey directory");
        
        // Create workflows directory
        var workflowsDir = Path.Combine(projectDir, "workflows");
        Directory.CreateDirectory(workflowsDir);
        logger.LogInformation("Created workflows directory");
        
        // Create .gitignore
        var gitignorePath = Path.Combine(projectDir, ".gitignore");
        if (!File.Exists(gitignorePath))
        {
            var gitignoreContent = GenerateGitignore();
            await File.WriteAllTextAsync(gitignorePath, gitignoreContent);
            logger.LogInformation("Created .gitignore");
        }
        
        logger.LogInformation("Project initialized successfully in {ProjectDir}", projectDir);
    }
    
    static string GenerateConfigJson(string template)
    {
        return $$"""
        {
          "version": "1.0.0",
          "network": {
            "name": "local",
            "endpoint": "http://localhost:50051",
            "timeout": 30
          },
          "credentials": {
            "credentialSource": "env"
          },
          "workflows": {
            "directory": "./workflows",
            "autoValidate": true
          },
          "testing": {
            "timeout": 60,
            "parallel": true,
            "coverage": false
          },
          "plugins": [],
          "logging": {
            "level": "info",
            "format": "text"
          }
        }
        """;
    }
    
    static string GenerateGitignore()
    {
        return """
        # MameyNode
        .mamey/
        *.log
        
        # Credentials
        credentials/*.json
        !credentials/.gitkeep
        
        # Build
        bin/
        obj/
        """;
    }
}
