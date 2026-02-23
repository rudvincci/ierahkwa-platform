using Mamey.Postgres;

namespace Pupitre.Notifications.Infrastructure.EF;

internal class NotificationUnitOfWork : PostgresUnitOfWork<NotificationDbContext>, INotificationUnitOfWork
{
    public NotificationUnitOfWork(NotificationDbContext dbContext) : base(dbContext)
    {
    }
}
