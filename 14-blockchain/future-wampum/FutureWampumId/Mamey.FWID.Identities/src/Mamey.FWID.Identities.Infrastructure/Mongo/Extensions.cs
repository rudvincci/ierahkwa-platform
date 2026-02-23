using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.Persistence.MongoDB;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.FWID.Identities.Infrastructure.Mongo.Options;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mamey.FWID.Identities.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        // Configure Guid representation for MongoDB
        // This must be done before creating any MongoClient instances
        // Register Guid serializer with Standard representation to avoid serialization errors
        // Use try-catch to handle cases where serializer is already registered (e.g., in tests)
        try
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }
        catch (BsonSerializationException)
        {
            // Serializer already registered, ignore
        }

        // Register standalone Mongo repositories (not as interfaces, composites will use them)
        builder.Services.AddScoped<IdentityMongoRepository>();
        
        // Register MongoDB sync options
        builder.Services.Configure<MongoSyncOptions>(builder.Configuration.GetSection("mongoSync"));

        // Register Postgres repository needed by sync service
        builder.Services.AddScoped<IdentityPostgresRepository>();
        
        // Register background sync service to sync from Postgres to MongoDB
        builder.Services.AddHostedService<IdentityMongoSyncService>();

        return builder
            .AddMongo()
            .AddMongoRepository<IdentityDocument, Guid>("identity");
    }
}
