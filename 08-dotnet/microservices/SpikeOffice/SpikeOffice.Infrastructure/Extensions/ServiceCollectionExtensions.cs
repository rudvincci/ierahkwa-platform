using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpikeOffice.Core.Interfaces;
using SpikeOffice.Infrastructure.Data;
using SpikeOffice.Infrastructure.Middleware;
using SpikeOffice.Infrastructure.Services;

namespace SpikeOffice.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpikeOfficeInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("Default") ?? "Data Source=spikeoffice.db";
        services.AddDbContext<SpikeOfficeDbContext>(o => o.UseSqlite(conn));

        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }

    public static IApplicationBuilder UseSpikeOfficeTenantResolution(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantResolutionMiddleware>();
    }
}
