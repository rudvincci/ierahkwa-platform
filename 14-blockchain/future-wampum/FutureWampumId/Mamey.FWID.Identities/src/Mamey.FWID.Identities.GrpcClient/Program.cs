// ============================================================================
// IMPORTANT: This file is for DEMO/TESTING purposes only.
// ============================================================================
// The GrpcClient project is primarily a LIBRARY to be referenced by other
// services that need to call the Identities API via gRPC.
//
// To use this client in your service:
// 1. Add a project reference to Mamey.FWID.Identities.GrpcClient
// 2. Call AddIdentitiesGrpcInfrastructure() in your Program.cs
// 3. Inject BiometricServiceClient where needed
//
// See README.md for detailed usage instructions.
// ============================================================================

using Mamey.Logging;
using Mamey.Secrets.Vault;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.GrpcClient;

/// <summary>
/// Demo/Test console application for testing the gRPC client.
/// This is NOT the primary use case - the GrpcClient should be used as a library.
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure Mamey services
        builder.Services
            .AddMamey()
            .AddIdentitiesGrpcInfrastructure(args);
        
        // Configure logging (Vault is optional for GrpcClient - only needed if accessing secrets)
        builder.Host.UseLogging();
        
        // Only use Vault if configured (optional for GrpcClient)
        var configuration = builder.Configuration;
        var vaultSection = configuration.GetSection("vault");
        if (vaultSection.Exists() && !string.IsNullOrWhiteSpace(vaultSection["authType"]))
        {
            builder.Host.UseVault();
        }
        
        // Configure the web app to use a different port than the API (if needed)
        // The GrpcClient doesn't need to listen on a port, but if it does, use a different one
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5101); // Different port from API (5001)
        });
        
        var app = builder.Build();
        
        // Simple health check endpoint
        app.MapGet("/", () => "Identities gRPC Client is running. Use the client methods to call the service.");
        app.MapGet("/health", () => "Healthy");
        
        await app.RunAsync();
    }
}

