using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mamey.Authentik;
using Mamey.Authentik.Services;

namespace Mamey.Authentik.Samples.UserManagement;

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
            
            // Configure caching
            options.CacheOptions.Enabled = true;
            options.CacheOptions.DefaultTtl = TimeSpan.FromMinutes(10);
            
            // Configure retry policy
            options.RetryPolicy.MaxRetries = 3;
            options.RetryPolicy.UseExponentialBackoff = true;
        });

        // Add hosted service for demonstration
        services.AddHostedService<UserManagementService>();
    })
    .Build();

        await host.RunAsync();
    }
}

public class UserManagementService : BackgroundService
{
    private readonly IAuthentikClient _authentik;
    private readonly ILogger<UserManagementService> _logger;

    public UserManagementService(
        IAuthentikClient authentik,
        ILogger<UserManagementService> logger)
    {
        _authentik = authentik;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting user management demonstration");

        try
        {
            // Example: Get a user
            // var user = await _authentik.Core.GetUserAsync("user-id", stoppingToken);
            // _logger.LogInformation("Retrieved user: {UserId}", user?.ToString());

            // Example: List users with pagination
            // var users = await _authentik.Core.ListUsersAsync(page: 1, pageSize: 20, stoppingToken);
            // _logger.LogInformation("Retrieved {Count} users", users.Results.Count);

            _logger.LogInformation("User management demonstration completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user management demonstration");
        }
    }
}
