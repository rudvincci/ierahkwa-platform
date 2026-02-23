using Mamey.ISO.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ISO3166;

public static class Extensions
{
    public static IServiceCollection AddISO3166(this IServiceCollection services)
    {
        services.AddScoped<IISO3166Service, ISO3166Service>();

        return services;
    }
    public static  IApplicationBuilder UseISO3166(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        var iso3166Service = scope.ServiceProvider.GetService<IISO3166Service>();
        iso3166Service.InitializeAsync().GetAwaiter().GetResult();

        return builder;
    }
}

