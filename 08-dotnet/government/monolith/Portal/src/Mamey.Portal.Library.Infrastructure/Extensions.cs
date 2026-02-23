using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey;
using Mamey.Portal.Library.Infrastructure.Persistence;
using Mamey.Portal.Library.Infrastructure.Persistence.Repositories;
using Mamey.Portal.Library.Infrastructure.Services;
using Mamey.Portal.Library.Application.Services;
using Mamey.Portal.Library.Domain.Repositories;

namespace Mamey.Portal.Library.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddLibraryInfrastructure(
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
        builder.Services.AddDbContext<LibraryDbContext>(options =>
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
        services.AddScoped<ILibraryService, LibraryService>();
        services.AddScoped<ILibraryStore, LibraryStore>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<ILibraryItemRepository, PostgresLibraryItemRepository>();
    }
}
