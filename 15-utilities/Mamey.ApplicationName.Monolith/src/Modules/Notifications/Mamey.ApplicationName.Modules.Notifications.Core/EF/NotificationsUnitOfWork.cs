using Mamey.Postgres;

namespace Mamey.ApplicationName.Modules.Notifications.Core.EF
{
    internal class NotificationsUnitOfWork : PostgresUnitOfWork<NotificationsDbContext>, INotificationsUnitOfWork
    {
        public NotificationsUnitOfWork(NotificationsDbContext dbContext) : base(dbContext)
        {
        }
    }
}