using System;
using Mamey.CQRS.Queries;
using Pupitre.Notifications.Application.DTO;
using Pupitre.Notifications.Application.Queries;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using MongoDB.Driver;

namespace Pupitre.Notifications.Infrastructure.Mongo.Queries;

internal sealed class GetNotificationHandler : IQueryHandler<GetNotification, NotificationDetailsDto>
{
    private const string collectionName = "notifications";
    private readonly IMongoDatabase _database;

    public GetNotificationHandler(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<NotificationDetailsDto> HandleAsync(GetNotification query, CancellationToken cancellationToken = default)
    {
        var document = await _database.GetCollection<NotificationDocument>(collectionName)
            .Find(r => r.Id == query.Id)
            .SingleOrDefaultAsync();

        return document?.AsDetailsDto();
    }
}


