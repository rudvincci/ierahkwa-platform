using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Notifications.Domain.Repositories;
using Pupitre.Notifications.Infrastructure.Mongo.Documents;
using Pupitre.Notifications.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Notifications.Infrastructure.Mongo.Services;
namespace Pupitre.Notifications.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<INotificationRepository, NotificationMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<NotificationDocument, Guid>($"notifications");
    }
}

