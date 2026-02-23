using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.Government.Modules.Notifications.Core.Mongo.Repositories;

internal class SMSNotificationMongoRepository : ISMSNotificationRepository
{
    private readonly IMongoRepository<SMSNotificationDocument, Guid> _mongoRepository;

    public SMSNotificationMongoRepository(IMongoRepository<SMSNotificationDocument, Guid> mongoRepository)
    {
        _mongoRepository = mongoRepository;
    }
    public async Task AddAsync(SMSNotification notification)
        => await _mongoRepository.AddAsync(new SMSNotificationDocument(notification));

    public async Task<SMSNotification?> GetAsync(Guid id)
    {
        var document = await _mongoRepository.GetAsync(id);
        return document?.AsEntity();
    }

    public async Task UpdateAsync(SMSNotification notification)
        => await _mongoRepository.UpdateAsync(new SMSNotificationDocument(notification));
}