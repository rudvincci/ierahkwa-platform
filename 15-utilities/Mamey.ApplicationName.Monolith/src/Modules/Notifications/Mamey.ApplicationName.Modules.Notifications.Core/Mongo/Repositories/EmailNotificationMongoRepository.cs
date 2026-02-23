using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;
using Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Repositories;

internal class EmailNotificationMongoRepository : IEmailNotificationRepository
{
    private readonly IMongoRepository<EmailNotificationDocument, Guid> _mongoRepository;

    public EmailNotificationMongoRepository(IMongoRepository<EmailNotificationDocument, Guid> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }
    public async Task AddAsync(EmailNotification notification)
        => await _mongoRepository.AddAsync(new EmailNotificationDocument(notification));

    public async Task<EmailNotification?> GetAsync(Guid id)
    {
        var document = await _mongoRepository.GetAsync(id);
        return document?.AsEntity();
    }

    public async Task UpdateAsync(EmailNotification notification)
        => await _mongoRepository.UpdateAsync(new EmailNotificationDocument(notification));
}