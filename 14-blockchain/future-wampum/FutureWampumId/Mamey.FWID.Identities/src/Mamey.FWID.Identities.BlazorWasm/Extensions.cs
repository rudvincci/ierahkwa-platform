using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey;
using Mamey.BlazorWasm.Routing;
using Mamey.FWID.Identities.BlazorWasm.Services;

namespace Mamey.FWID.Identities.BlazorWasm;

public static class Extensions
{
    private const string RegistryName = "fwid-identities-blazorwasm";

    public static IMameyBuilder AddIdentitiesBlazorWasm(this IMameyBuilder builder, string? apiBaseUrl = null)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        var baseUrl = apiBaseUrl ?? builder.Configuration.GetValue<string>("FutureWampum:Identities:ApiBaseUrl") ?? "http://localhost:5001";

        builder.Services.AddHttpClient<IIdentityService, IdentityService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        // Add route service
        builder.Services.AddScoped<IRouteService, IdentityRouteService>();

        // Add biometric capture service
        builder.Services.AddScoped<IBiometricCaptureService, BiometricCaptureService>();

        return builder;
    }

    public static IServiceCollection AddIdentitiesBlazorWasm(this IServiceCollection services, IConfiguration configuration)
    {
        var baseUrl = configuration.GetValue<string>("FutureWampum:Identities:ApiBaseUrl") ?? "http://localhost:5001";

        services.AddHttpClient<IIdentityService, IdentityService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddScoped<IRouteService, IdentityRouteService>();

        // Add biometric capture service
        services.AddScoped<IBiometricCaptureService, BiometricCaptureService>();

        return services;
    }

    public static IApplicationBuilder UseIdentitiesBlazorWasm(this IApplicationBuilder builder)
    {
        return builder;
    }
}
