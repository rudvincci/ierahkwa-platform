using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey;
using Mamey.Portal.Tenant.Infrastructure.Persistence;
using Mamey.Portal.Tenant.Infrastructure.Persistence.Repositories;
using Mamey.Portal.Tenant.Infrastructure.Services;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Domain.Repositories;

namespace Mamey.Portal.Tenant.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddTenantInfrastructure(
        this IMameyBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PortalDb")
            ?? "Host=localhost;Database=mamey_portal_dev;Username=postgres;Password=postgres";

        // 1. Ensure logging is registered
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        // 2. Register DbContext
        builder.Services.AddDbContext<TenantDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            var enableSensitiveLogging = builder.Configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging");
            if (enableSensitiveLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // 3. Register Application Services
        RegisterApplicationServices(builder.Services);
        RegisterRepositories(builder.Services);

        return builder;
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ITenantOnboardingService, TenantOnboardingService>();
        services.AddScoped<ITenantOnboardingStore, TenantOnboardingStore>();
        services.AddScoped<ITenantUserMappingService, TenantUserMappingService>();
        services.AddScoped<ITenantUserMappingStore, TenantUserMappingStore>();
        services.AddScoped<ITenantUserInviteService, TenantUserInviteService>();
        services.AddScoped<ITenantUserInviteStore, TenantUserInviteStore>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, PostgresTenantRepository>();
        services.AddScoped<ITenantSettingsRepository, PostgresTenantSettingsRepository>();
        services.AddScoped<IDocumentNamingRepository, PostgresDocumentNamingRepository>();
        services.AddScoped<IDocumentTemplateRepository, PostgresDocumentTemplateRepository>();
        services.AddScoped<IUserMappingRepository, PostgresUserMappingRepository>();
        services.AddScoped<IUserInviteRepository, PostgresUserInviteRepository>();
    }
}
