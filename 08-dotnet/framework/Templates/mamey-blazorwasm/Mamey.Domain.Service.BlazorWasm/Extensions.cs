using Mamey.Domain.Service.BlazorWasm.ApiClients;
using Mamey.Domain.Service.BlazorWasm.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Mamey.Domain.Service.BlazorWasm;

public static class Extensions
{
    public static IMameyBuilder AddServiceBlazorWasm(this IMameyBuilder builder)
    {
        builder.Services
            .AddServices()
            .AddApiClients(builder.Configuration);
        
        return builder;
    }

    public static async Task<WebApplication> UseServiceBlazorWasmAsync(this WebApplication app)
    {
        return app;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IServiceService, ServiceService>();
        return services;
    }
    
    private static IServiceCollection AddApiClients(this IServiceCollection services, IConfiguration configuration)
    {
        // Get API base URL from configuration
        // Format: "ApiBaseUrl:Domain:Service" or fallback to parameter value
        var configKey = $"ApiBaseUrl:Domain:Service";
        var configuredUrl = configuration.GetValue<string>(configKey) ?? "ApiBaseUrl";
        
        services.AddRefitClient<IServiceApiClient>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(configuredUrl);
            });
        
        return services;
    }
}

