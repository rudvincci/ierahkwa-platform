using Mamey.Postgres;

namespace Mamey.Government.Modules.Notifications.Core.EF
{
    internal class NotificationsUnitOfWork : PostgresUnitOfWork<NotificationsDbContext>, INotificationsUnitOfWork
    {
        private readonly NotificationsDbContext _dbContext;

        public NotificationsUnitOfWork(NotificationsDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}