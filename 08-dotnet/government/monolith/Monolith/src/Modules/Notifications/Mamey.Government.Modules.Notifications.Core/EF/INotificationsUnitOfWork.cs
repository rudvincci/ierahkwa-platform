using Mamey.Persistence.SQL;

namespace Mamey.Government.Modules.Notifications.Core.EF
{
    internal interface INotificationsUnitOfWork : IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}