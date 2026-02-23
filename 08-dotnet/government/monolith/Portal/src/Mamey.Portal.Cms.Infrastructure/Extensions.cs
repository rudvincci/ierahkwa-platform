using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey;
using Mamey.Portal.Cms.Infrastructure.Persistence;
using Mamey.Portal.Cms.Infrastructure.Persistence.Repositories;
using Mamey.Portal.Cms.Infrastructure.Services;
using Mamey.Portal.Cms.Application.Services;
using Mamey.Portal.Cms.Domain.Repositories;

namespace Mamey.Portal.Cms.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddCmsInfrastructure(
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
        builder.Services.AddDbContext<CmsDbContext>(options =>
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
        RegisterInfrastructureServices(builder.Services);

        return builder;
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        services.AddScoped<ICmsContentService, CmsContentService>();
        services.AddScoped<ICmsContentStore, CmsContentStore>();
        services.AddScoped<ICmsPageService, CmsPageService>();
        services.AddScoped<ICmsPageStore, CmsPageStore>();
    }

    private static void RegisterInfrastructureServices(IServiceCollection services)
    {
        services.AddSingleton<ICmsHtmlSanitizer, Mamey.Portal.Cms.Application.Services.CmsHtmlSanitizer>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<ICmsNewsRepository, PostgresCmsNewsRepository>();
        services.AddScoped<ICmsPageRepository, PostgresCmsPageRepository>();
    }
}
