using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.FWID.Notifications.Infrastructure.PostgreSQL.Repositories;
using Mamey.Microservice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Notifications.Infrastructure.PostgreSQL;

internal static class Extensions
{
    public static IMameyBuilder AddPostgresDb(this IMameyBuilder builder)
    {
        var options = builder.Services.GetOptions<PostgresOptions>("postgres");
        builder.Services.AddDbContext<NotificationDbContext>(db =>
            db.UseNpgsql(options.ConnectionString));

        builder.Services.AddScoped<INotificationRepository, NotificationPostgresRepository>();

        return builder;
    }

    public static IApplicationBuilder UseNotificationPostgres(this IApplicationBuilder builder)
    {
        var dbContext = builder.ApplicationServices.GetRequiredService<NotificationDbContext>();
        dbContext.Database.Migrate();
        return builder;
    }
}

