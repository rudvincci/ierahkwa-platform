using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Authentik;
using Mamey.Authentik.Services;

namespace Mamey.Authentik.Samples.FlowExecution;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Configure Authentik
        services.AddAuthentik(options =>
        {
            options.BaseUrl = context.Configuration["Authentik:BaseUrl"] 
                ?? throw new InvalidOperationException("Authentik:BaseUrl is required");
            options.ApiToken = context.Configuration["Authentik:ApiToken"] 
                ?? throw new InvalidOperationException("Authentik:ApiToken is required");
        });

        // Add hosted service for demonstration
        services.AddHostedService<FlowExecutionService>();
    })
    .Build();

        await host.RunAsync();
    }
}

public class FlowExecutionService : BackgroundService
{
    private readonly IAuthentikClient _authentik;
    private readonly ILogger<FlowExecutionService> _logger;

    public FlowExecutionService(
        IAuthentikClient authentik,
        ILogger<FlowExecutionService> logger)
    {
        _authentik = authentik;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting flow execution demonstration");

        try
        {
            // Example: Flow operations
            // var flows = await _authentik.Flows.ListFlowsAsync(stoppingToken);
            // _logger.LogInformation("Retrieved {Count} flows", flows.Results.Count);

            _logger.LogInformation("Flow execution demonstration completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during flow execution demonstration");
        }
    }
}
