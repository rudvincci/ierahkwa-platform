using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Authentik;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure Authentik
        services.AddAuthentik(options =>
        {
            options.BaseUrl = context.Configuration["Authentik:BaseUrl"] 
                ?? "https://authentik.company.com";
            options.ApiToken = context.Configuration["Authentik:ApiToken"] 
                ?? throw new InvalidOperationException("Authentik:ApiToken is required");
        });

        // Add hosted service for demonstration
        services.AddHostedService<AuthentikSampleService>();
    })
    .Build();

await host.RunAsync();

public class AuthentikSampleService : BackgroundService
{
    private readonly IAuthentikClient _authentik;
    private readonly ILogger<AuthentikSampleService> _logger;

    public AuthentikSampleService(
        IAuthentikClient authentik,
        ILogger<AuthentikSampleService> logger)
    {
        _authentik = authentik;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Authentik client initialized");

        try
        {
            // Example usage - methods will be available after code generation
            // var users = await _authentik.Core.GetUsersAsync(cancellationToken: stoppingToken);
            // _logger.LogInformation("Retrieved {Count} users", users.Count);

            _logger.LogInformation("Sample completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sample execution");
        }
    }
}
