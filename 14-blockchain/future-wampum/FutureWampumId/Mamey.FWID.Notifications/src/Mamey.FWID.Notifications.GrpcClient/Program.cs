using Mamey.Logging;
using Mamey.Secrets.Vault;
using Microsoft.AspNetCore;

namespace Mamey.FWID.Notifications.GrpcClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure Mamey services
        builder.Services
            .AddMamey()
            .AddNotificationsGrpcInfrastructure(args);
        
        builder.Host.ConfigureWebHostDefaults(config =>
        {
            ((IHostBuilder)config)
                .UseLogging()
                .UseVault();
        });
        
        var app = builder.Build();
        
        // Simple health check endpoint
        app.MapGet("/", () => "Notifications gRPC Client is running. Use the client methods to call the service.");
        app.MapGet("/health", () => "Healthy");
        
        await app.RunAsync();
    }
}







