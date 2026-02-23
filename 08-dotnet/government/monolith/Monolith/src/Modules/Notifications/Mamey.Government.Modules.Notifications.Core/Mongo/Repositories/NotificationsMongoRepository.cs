using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Government.Modules.Notifications.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.Government.Modules.Notifications.Core.Mongo.Repositories
{
    internal class NotificationsMongoRepository : INotificationRepository
    {
        private readonly IMongoRepository<NotificationDocument, Guid> _repository;

        public NotificationsMongoRepository(IMongoRepository<NotificationDocument, Guid> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
            => await _repository.AddAsync(new NotificationDocument(notification)); 

        public async Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
            => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

        public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
            => await _repository.ExistsAsync(c => c.Id == id.Value);

        public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
        {
            var notification = await _repository.GetAsync(id.Value);
            return notification?.AsEntity();
        }

        public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
            => await _repository.UpdateAsync(new NotificationDocument(notification));

        public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id.Value);
        }
    }
}
