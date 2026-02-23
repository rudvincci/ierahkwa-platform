using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Authentik;
using Mamey.Authentik.Services;

namespace Mamey.Authentik.Samples.OAuth2Flow;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure Authentik with OAuth2 credentials
        services.AddAuthentik(options =>
        {
            options.BaseUrl = context.Configuration["Authentik:BaseUrl"] 
                ?? throw new InvalidOperationException("Authentik:BaseUrl is required");
            options.ClientId = context.Configuration["Authentik:ClientId"] 
                ?? throw new InvalidOperationException("Authentik:ClientId is required");
            options.ClientSecret = context.Configuration["Authentik:ClientSecret"] 
                ?? throw new InvalidOperationException("Authentik:ClientSecret is required");
        });

        // Add hosted service for demonstration
        services.AddHostedService<OAuth2FlowService>();
    })
    .Build();

        await host.RunAsync();
    }
}

public class OAuth2FlowService : BackgroundService
{
    private readonly IAuthentikClient _authentik;
    private readonly ILogger<OAuth2FlowService> _logger;

    public OAuth2FlowService(
        IAuthentikClient authentik,
        ILogger<OAuth2FlowService> logger)
    {
        _authentik = authentik;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting OAuth2 flow demonstration");

        try
        {
            // Example: OAuth2 operations
            // The OAuth2 service will automatically handle token acquisition and refresh
            // when using ClientId/ClientSecret authentication
            
            _logger.LogInformation("OAuth2 flow demonstration completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OAuth2 flow demonstration");
        }
    }
}
