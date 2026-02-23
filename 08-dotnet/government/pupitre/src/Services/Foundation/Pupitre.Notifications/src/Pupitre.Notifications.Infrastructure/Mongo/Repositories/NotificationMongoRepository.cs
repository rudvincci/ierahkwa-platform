using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Notifications.Domain.Repositories;
using Pupitre.Notifications.Domain.Entities;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Notifications.Infrastructure.Mongo.Repositories;

internal class NotificationMongoRepository : INotificationRepository
{
    private readonly IMongoRepository<NotificationDocument, Guid> _repository;

    public NotificationMongoRepository(IMongoRepository<NotificationDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new NotificationDocument(notification));

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new NotificationDocument(notification));
    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Notification> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var notification = await _repository.GetAsync(id.Value);
        return notification?.AsEntity();
    }
    public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



