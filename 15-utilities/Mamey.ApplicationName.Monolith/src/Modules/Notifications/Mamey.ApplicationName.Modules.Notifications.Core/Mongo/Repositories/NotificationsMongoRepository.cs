using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Types;
using Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Repositories
{
    internal class NotificationsMongoRepository : INotificationRepository
    {
        private readonly IMongoRepository<NotificationDocument, Guid> _repository;

        public NotificationsMongoRepository(IMongoRepository<NotificationDocument, Guid> repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Notification notification)
            => await _repository.AddAsync(new NotificationDocument(notification)); 

        public async Task<IReadOnlyList<Notification>> BrowseAsync()
            => (await _repository.FindAsync(_ => true))
            .Select(c => c.AsEntity())
            .ToList();

        public async Task<bool> ExistsAsync(Guid id)
            => await _repository.ExistsAsync(c => c.Id == id);

        public async Task<Notification> GetAsync(NotificationId id)
        {
            var notification = await _repository.GetAsync(id);
            return notification?.AsEntity();
        }

        public async Task UpdateAsync(Notification notification)
            => await _repository.UpdateAsync(new NotificationDocument(notification));
    }
}
