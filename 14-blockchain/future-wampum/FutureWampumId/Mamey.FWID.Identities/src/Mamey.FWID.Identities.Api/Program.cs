using Mamey;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.WebApi.CQRS.Builders;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Mamey.FWID.Identities.Infrastructure;
using Mamey.Logging;
using Mamey.Net;
using Mamey.Secrets.Vault;
using Mamey.FWID.Identities.Api;
using Mamey.Microservice.Infrastructure.Diagnostics;

namespace Mamey.FWID.Identities.Api;

public class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args)
            .Build()
            .RunAsync();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args).
          ConfigureWebHostDefaults(webhostBuilder =>
          {
              webhostBuilder.ConfigureServices((context, services) =>
                {
                    services
                        .AddMamey()
                        .AddWebApi()
                        .AddInfrastructure(context.Configuration);
                    
                    // Explicitly register gRPC services in DI (required before MapGrpcService)
                    // This ensures all dependencies are resolved correctly
                    services.AddScoped<Infrastructure.Grpc.BiometricGrpcService>();
                    services.AddScoped<Infrastructure.Grpc.PermissionSyncGrpcService>();
                })
              .Configure(app =>
                {
                    // UseInfrastructure() calls UseSharedInfrastructure() which handles:
                    // - UseRouting()
                    // - UseMultiAuth()
                    // - UseAuthorization()
                    // - Other infrastructure middleware
                    app.UseInfrastructure();
                    
                    // Register all endpoints using UseEndpoints
                    // UseInfrastructure() already called UseRouting(), so we can use UseEndpoints() directly
                    app.UseEndpoints(endpoints =>
                    {
                        // Use the Dispatch extension to get a dispatcher builder within UseEndpoints
                        // This avoids calling UseDispatcherEndpoints() which would call UseRouting() again
                        var definitions = app.ApplicationServices.GetRequiredService<Mamey.WebApi.WebApiEndpointDefinitions>();
                        var endpointsBuilder = new Mamey.WebApi.EndpointsBuilder(endpoints, definitions);
                        var dispatcher = endpointsBuilder.Dispatch(d => d);
                        
                        // Add root endpoint via dispatcher builder
                        dispatcher.Get("/", async (HttpContext ctx) =>
                        {
                            var appOptions = ctx.RequestServices.GetService<AppOptions>();
                            await ctx.Response.WriteAsJsonAsync(new { name = appOptions?.Name ?? "Mamey.FWID.Identities" });
                        }, auth: false);
                        
                        // Add identity routes
                        dispatcher.AddIdentityRoutes();

                        
                        // Add additional endpoints (health, gRPC)
                        endpoints.MapHealthEndpoint();
                        endpoints.MapGrpcService<Infrastructure.Grpc.BiometricGrpcService>();
                        endpoints.MapGrpcService<Infrastructure.Grpc.PermissionSyncGrpcService>();
                    });
                });
          })
          .UseLogging()
          .UseVault();
}

