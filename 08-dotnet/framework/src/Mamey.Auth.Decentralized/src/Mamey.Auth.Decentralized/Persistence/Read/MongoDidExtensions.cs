using MongoDB.Driver;
using Mamey.Auth.Decentralized.Persistence.Read.Repositories;

namespace Mamey.Auth.Decentralized.Persistence.Read;

/// <summary>
/// Extension methods for MongoDB DID persistence
/// </summary>
public static class MongoDidExtensions
{
    /// <summary>
    /// Adds MongoDB DID persistence services
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMongoDidPersistence(this IServiceCollection services, Action<MongoDidOptions>? configureOptions = null)
    {
        var options = new MongoDidOptions();
        configureOptions?.Invoke(options);

        services.Configure<MongoDidOptions>(opt =>
        {
            opt.ConnectionString = options.ConnectionString;
            opt.DatabaseName = options.DatabaseName;
            opt.DidDocumentsCollectionName = options.DidDocumentsCollectionName;
            opt.VerificationMethodsCollectionName = options.VerificationMethodsCollectionName;
            opt.ServiceEndpointsCollectionName = options.ServiceEndpointsCollectionName;
            opt.ProofsCollectionName = options.ProofsCollectionName;
            opt.CreateIndexes = options.CreateIndexes;
            opt.TimeoutSeconds = options.TimeoutSeconds;
        });

        services.AddSingleton<IMongoClient>(provider =>
        {
            var clientSettings = MongoClientSettings.FromConnectionString(options.ConnectionString);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            return new MongoClient(clientSettings);
        });

        services.AddSingleton<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.DatabaseName);
        });

        services.AddSingleton<MongoDidDbContext>();

        // Register repositories
        services.AddScoped<IDidDocumentReadRepository>(provider =>
        {
            var context = provider.GetRequiredService<MongoDidDbContext>();
            var logger = provider.GetRequiredService<ILogger<MongoDidDocumentReadRepository>>();
            return new MongoDidDocumentReadRepository(context.DidDocuments, logger);
        });

        services.AddScoped<IVerificationMethodReadRepository>(provider =>
        {
            var context = provider.GetRequiredService<MongoDidDbContext>();
            var logger = provider.GetRequiredService<ILogger<MongoVerificationMethodReadRepository>>();
            return new MongoVerificationMethodReadRepository(context.VerificationMethods, logger);
        });

        services.AddScoped<IServiceEndpointReadRepository>(provider =>
        {
            var context = provider.GetRequiredService<MongoDidDbContext>();
            var logger = provider.GetRequiredService<ILogger<MongoServiceEndpointReadRepository>>();
            return new MongoServiceEndpointReadRepository(context.ServiceEndpoints, logger);
        });

        return services;
    }

    /// <summary>
    /// Configures MongoDB DID persistence with Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="configureOptions">Action to configure options</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddMongoDidPersistence(this IMameyBuilder builder, Action<MongoDidOptions>? configureOptions = null)
    {
        builder.Services.AddMongoDidPersistence(configureOptions);
        return builder;
    }
}
