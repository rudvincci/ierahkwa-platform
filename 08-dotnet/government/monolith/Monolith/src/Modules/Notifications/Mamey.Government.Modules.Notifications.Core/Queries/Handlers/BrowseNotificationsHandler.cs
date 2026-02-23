using Mamey.Government.Modules.Notifications.Core.DTO;
using Mamey.Government.Modules.Notifications.Core.Mongo.Documents;
using Mamey.CQRS.Queries;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mamey.Government.Modules.Notifications.Core.Queries.Handlers;

internal class BrowseNotificationsHandler : IQueryHandler<BrowseNotifications, PagedResult<NotificationDto>>
{
    private readonly ILogger<BrowseNotificationsHandler> _logger;
    private readonly string collectionName = $"{Mongo.Extensions.Schema}.notifications";
        private readonly IMongoDatabase _database;
    public BrowseNotificationsHandler()
    {
    }

    public async Task<PagedResult<NotificationDto>> HandleAsync(BrowseNotifications query, CancellationToken cancellationToken = default)
    {
        var filter = await GetFilterAsync(query);
        var sort = GetSort(query);
        var notificationsCollection = _database.GetCollection<NotificationDocument>(collectionName);
        var filteredResults = await notificationsCollection.AggregateByPageAsync(
            filter,
            sort,
            page: query.Page,
            pageSize: query.TotalResults);
        return filteredResults.Data.Select(c=> c.AsDto()).Paginate(query);
    }
    private SortDefinition<NotificationDocument> GetSort(BrowseNotifications query)
    {
        var sortBuilder = Builders<NotificationDocument>.Sort;
        SortDefinition<NotificationDocument> sort = sortBuilder.Ascending("id");
        if (!string.IsNullOrEmpty(query.SortOrder))
        {
            if (query.SortOrder.ToLower() == "asc")
            {
                sort = sortBuilder.Ascending(query.OrderBy);
            }
            else if (query.SortOrder.ToLower() == "desc")   
            {
                sort = sortBuilder.Descending(query.OrderBy);
            }
        }
        return sort;
    }
    private async Task<FilterDefinition<NotificationDocument>> GetFilterAsync(BrowseNotifications query)
    {
        
        var filterBuilder = Builders<NotificationDocument>.Filter;
        FilterDefinition<NotificationDocument> filter = query.ShowRead ? filterBuilder.Empty : filterBuilder.Eq(c => c.IsRead, true);


        if (query.UserId != null && query.UserId != Guid.Empty)
        {
            
            var userIdFilter1 = Builders<NotificationDocument>.Filter.ElemMatch<NotificationDocument>("userId",
                filterBuilder.Eq(c => c.UserId, query.UserId));

            var userIdFilter = filterBuilder.Regex("userId", new BsonRegularExpression($"/{query.UserId}/is"));

            var stringFilter = userIdFilter;

            if (filter == filterBuilder.Empty)
            {
                filter = stringFilter;
            }
            else
            {
                filter = filter & stringFilter;
            }
        }

        
        
        return filter;
    }
}