using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.FWID.Notifications.Infrastructure.Mongo.Documents;
using Mamey.Microservice.Infrastructure.Mongo;
using MongoDB.Driver;

namespace Mamey.FWID.Notifications.Infrastructure.Mongo.Repositories;

/// <summary>
/// MongoDB repository implementation for Notification read model.
/// </summary>
internal class NotificationMongoRepository : INotificationRepository
{
    private readonly IMongoRepository<NotificationDocument, string> _repository;

    public NotificationMongoRepository(IMongoRepository<NotificationDocument, string> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value.ToString(), cancellationToken);
        return document?.ToEntity();
    }

    public async Task<IEnumerable<Notification>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(
            Builders<NotificationDocument>.Filter.Eq(d => d.IdentityId, identityId.Value.ToString()),
            cancellationToken);
        return documents.Select(d => d.ToEntity());
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        var document = new NotificationDocument
        {
            Id = notification.Id.Value.ToString(),
            IdentityId = notification.IdentityId.Value.ToString(),
            Title = notification.Title,
            Description = notification.Description,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            Status = notification.Status.ToString(),
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            ReadAt = notification.ReadAt,
            IsRead = notification.IsRead,
            RelatedEntityType = notification.RelatedEntityType,
            RelatedEntityId = notification.RelatedEntityId?.ToString()
        };
        await _repository.AddAsync(document, cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        var document = new NotificationDocument
        {
            Id = notification.Id.Value.ToString(),
            IdentityId = notification.IdentityId.Value.ToString(),
            Title = notification.Title,
            Description = notification.Description,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            Status = notification.Status.ToString(),
            CreatedAt = notification.CreatedAt,
            SentAt = notification.SentAt,
            ReadAt = notification.ReadAt,
            IsRead = notification.IsRead,
            RelatedEntityType = notification.RelatedEntityType,
            RelatedEntityId = notification.RelatedEntityId?.ToString()
        };
        await _repository.UpdateAsync(document, cancellationToken);
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value.ToString(), cancellationToken);
    }
}







