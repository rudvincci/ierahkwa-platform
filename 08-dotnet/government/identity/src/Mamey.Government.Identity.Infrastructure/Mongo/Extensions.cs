using Mamey.Persistence.MongoDB;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Documents;
using Mamey.Government.Identity.Infrastructure.Mongo.Options;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Services;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mamey.Government.Identity.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder, string sectionName = "mongoSync")
    {
        // Configure Guid representation for MongoDB
        // This must be done before creating any MongoClient instances
        // Register Guid serializer with Standard representation to avoid serialization errors
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Register MongoDB sync options
        builder.Services.Configure<MongoSyncOptions>(builder.Configuration.GetSection(sectionName));

        // Register standalone Mongo repositories (not as interfaces, composites will use them)
        builder.Services.AddScoped<UserMongoRepository>();
        builder.Services.AddScoped<RoleMongoRepository>();
        builder.Services.AddScoped<PermissionMongoRepository>();
        builder.Services.AddScoped<SubjectMongoRepository>();
        builder.Services.AddScoped<EmailConfirmationMongoRepository>();
        builder.Services.AddScoped<TwoFactorAuthMongoRepository>();
        builder.Services.AddScoped<MultiFactorAuthMongoRepository>();
        builder.Services.AddScoped<MfaChallengeMongoRepository>();
        builder.Services.AddScoped<CredentialMongoRepository>();
        
        // Register Postgres repositories needed by sync services
        builder.Services.AddScoped<UserPostgresRepository>();
        builder.Services.AddScoped<SubjectPostgresRepository>();
        builder.Services.AddScoped<RolePostgresRepository>();
        builder.Services.AddScoped<PermissionPostgresRepository>();
        builder.Services.AddScoped<EmailConfirmationPostgresRepository>();
        builder.Services.AddScoped<TwoFactorAuthPostgresRepository>();
        builder.Services.AddScoped<MultiFactorAuthPostgresRepository>();
        builder.Services.AddScoped<MfaChallengePostgresRepository>();
        builder.Services.AddScoped<CredentialPostgresRepository>();
        
        // Register background sync services to sync from Postgres to MongoDB
        builder.Services.AddHostedService<UserMongoSyncService>();
        builder.Services.AddHostedService<SubjectMongoSyncService>();
        builder.Services.AddHostedService<RoleMongoSyncService>();
        builder.Services.AddHostedService<PermissionMongoSyncService>();
        builder.Services.AddHostedService<CredentialMongoSyncService>();
        builder.Services.AddHostedService<EmailConfirmationMongoSyncService>();
        builder.Services.AddHostedService<TwoFactorAuthMongoSyncService>();
        builder.Services.AddHostedService<MultiFactorAuthMongoSyncService>();
        builder.Services.AddHostedService<MfaChallengeMongoSyncService>();

        return builder
            .AddMongo()
            .AddMongoRepository<SubjectDocument, Guid>("identity")
            .AddMongoRepository<UserDocument, Guid>("users")
            .AddMongoRepository<RoleDocument, Guid>("roles")
            .AddMongoRepository<PermissionDocument, Guid>("permissions")
            .AddMongoRepository<EmailConfirmationDocument, Guid>("email-confirmations")
            .AddMongoRepository<TwoFactorAuthDocument, Guid>("two-factor-auth")
            .AddMongoRepository<MultiFactorAuthDocument, Guid>("multi-factor-auth")
            .AddMongoRepository<MfaChallengeDocument, Guid>("mfa-challenges")
            .AddMongoRepository<CredentialDocument, Guid>("credentials");
    }
}

