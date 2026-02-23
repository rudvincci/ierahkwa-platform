using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Mongo;

public static class Extensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
      
        services.AddMongoRepository<Mongo.Documents.ApplicationDocument, Guid>("citizenship_applications.applications");
        services.AddMongoRepository<Domain.Entities.UploadedDocument, Guid>("citizenship_applications.uploaded_documents");
        // Register standalone Mongo repositories (not as interfaces, composites will use them)
        services.AddScoped<Mongo.Repositories.ApplicationMongoRepository>();
        services.AddScoped<Mongo.Repositories.UploadedDocumentMongoRepository>();
        
        // Register MongoDB sync options
        // builder.Services.Configure<MongoSyncOptions>(builder.Configuration.GetSection("mongoSync"));

        // Register Postgres repository needed by sync service
        // builder.Services.AddScoped<IdentityPostgresRepository>();
        
        // Register background sync service to sync from Postgres to MongoDB
        // builder.Services.AddHostedService<IdentityMongoSyncService>();
        return services;
        
    }
}