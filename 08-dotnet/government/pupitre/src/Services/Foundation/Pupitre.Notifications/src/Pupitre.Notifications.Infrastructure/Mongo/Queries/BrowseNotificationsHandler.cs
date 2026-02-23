using System;
using Mamey.CQRS.Queries;
using Mamey.Persistence.MongoDB;
using Pupitre.Notifications.Application.DTO;
using Pupitre.Notifications.Application.Queries;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Pupitre.Notifications.Infrastructure.Mongo.Queries;

internal sealed class BrowseNotificationsHandler : IQueryHandler<BrowseNotifications, PagedResult<NotificationDto>?>
{
    private readonly IMongoDatabase _database;

    public BrowseNotificationsHandler(IMongoDatabase database, IMongoRepository<NotificationDocument, Guid> repository)
    {
        _database = database;
    }

    public async Task<PagedResult<NotificationDto>?> HandleAsync(BrowseNotifications query, CancellationToken cancellationToken = default)
    {
        PagedResult<NotificationDocument> pagedResult;

        var notificationDocuments = _database.GetCollection<NotificationDocument>("notifications").AsQueryable();
        IEnumerable<NotificationDto>? dtos;

        if (IsEmptyQuery(query))
        {
            pagedResult = await notificationDocuments.PaginateAsync(query);
            return PagedResult<NotificationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
                pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages, pagedResult.TotalResults);
        }

        if (!string.IsNullOrEmpty(query.Name))
        {
            notificationDocuments = notificationDocuments
                .Where(c => c.Name.ToLower()
                    .Contains(query.Name.Trim().ToLowerInvariant()));
        }

        pagedResult = await notificationDocuments.PaginateAsync(query);

        return PagedResult<NotificationDto>.Create(pagedResult.Items.Select(c => c.AsDto()),
            pagedResult.CurrentPage, pagedResult.ResultsPerPage, pagedResult.TotalPages,
            pagedResult.TotalResults);
    }

    private bool IsEmptyQuery(BrowseNotifications query)
        => (string.IsNullOrEmpty(query.Name?.Trim())
            );
}



