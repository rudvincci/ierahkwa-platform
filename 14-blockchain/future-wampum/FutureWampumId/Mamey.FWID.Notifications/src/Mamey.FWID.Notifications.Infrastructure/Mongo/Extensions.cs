using Mamey.Persistence.MongoDB;
using Mamey.FWID.Notifications.Infrastructure.Mongo.Documents;
using Mamey.FWID.Notifications.Infrastructure.Mongo.Options;
using Mamey.FWID.Notifications.Infrastructure.Mongo.Repositories;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mamey.FWID.Notifications.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        // Configure Guid representation for MongoDB
        // This must be done before creating any MongoClient instances
        // Register Guid serializer with Standard representation to avoid serialization errors
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Register standalone Mongo repositories (not as interfaces, composites will use them)
        builder.Services.AddScoped<NotificationMongoRepository>();
        
        // Register MongoDB sync options
        builder.Services.Configure<MongoSyncOptions>(builder.Configuration.GetSection("mongoSync"));

        // Register Postgres repository needed by sync service
        builder.Services.AddScoped<PostgreSQL.Repositories.NotificationPostgresRepository>();
        
        // Register background sync service to sync from Postgres to MongoDB
        // Note: Sync service implementation can be added later if needed
        // builder.Services.AddHostedService<NotificationMongoSyncService>();

        return builder
            .AddMongo()
            .AddMongoRepository<NotificationDocument, string>("notification");
    }
}

