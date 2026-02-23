using Mamey.Government.Modules.Notifications.Core.DTO;
using Mamey.CQRS.Queries;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Mamey.Government.Modules.Notifications.Core.Queries.Handlers;

internal class ListNotificationsHandler : IQueryHandler<ListNotifications, List<NotificationDetailsDto>>
{
    private readonly ILogger<BrowseNotificationsHandler> _logger;
    private readonly string collectionName = $"{Mongo.Extensions.Schema}.notifications";
    private readonly IMongoDatabase _database;
    public ListNotificationsHandler()
    {
    }

    public async Task<List<NotificationDetailsDto>> HandleAsync(ListNotifications query, CancellationToken cancellationToken = default)
    {
        
        //var notificationsCollection = _database.GetCollection<NotificationDocument>(collectionName);
        
        //return await notificationsCollection.AsQueryable().Select(c=> c.AsDetailsDto()).ToListAsync();
        return await Task.FromResult(Extensions.MockNotificationService.GenerateMockNotifications(50));
    }
}