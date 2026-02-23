using Mamey.Persistence.MongoDB.Builders;
using Mamey.Persistence.MongoDB.Factories;
using Mamey.Persistence.MongoDB.Initializers;
using Mamey.Persistence.MongoDB.Repositories;
using Mamey.Persistence.MongoDB.Seeders;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Mamey.Persistence.MongoDB;

public static class Extensions
{
    // Helpful when dealing with integration testing
    private static bool _conventionsRegistered;
    private const string SectionName = "mongo";
    private const string RegistryName = "persistence.mongoDb";

    public static IMameyBuilder AddMongo(this IMameyBuilder builder, string sectionName = SectionName,
        Type seederType = null, bool registerConventions = true)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        var mongoOptions = builder.GetOptions<MongoDbOptions>(sectionName);
        return builder.AddMongo(mongoOptions, seederType, registerConventions);
    }

    public static IMameyBuilder AddMongo(this IMameyBuilder builder, Func<IMongoDbOptionsBuilder,
        IMongoDbOptionsBuilder> buildOptions, Type seederType = null, bool registerConventions = true)
    {
        var mongoOptions = buildOptions(new MongoDbOptionsBuilder()).Build();
        return builder.AddMongo(mongoOptions, seederType, registerConventions);
    }

    public static IMameyBuilder AddMongo(this IMameyBuilder builder, MongoDbOptions mongoOptions,
        Type seederType = null, bool registerConventions = true)
    {
        if (!builder.TryRegister(RegistryName))
        {
            return builder;
        }

        if (mongoOptions.SetRandomDatabaseSuffix)
        {
            var suffix = $"{Guid.NewGuid():N}";
            Console.WriteLine($"Setting a random MongoDB database suffix: '{suffix}'.");
            mongoOptions.Database = $"{mongoOptions.Database}_{suffix}";
        }

        builder.Services.AddSingleton(mongoOptions);
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var options = sp.GetRequiredService<MongoDbOptions>();
            var mongoClient = new MongoClient(options.ConnectionString);
            
            return mongoClient;
        });
        builder.Services.AddTransient(sp =>
        {
            var options = sp.GetRequiredService<MongoDbOptions>();
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(options.Database);
        });
        builder.Services.AddTransient<IMongoDbInitializer, MongoDbInitializer>();
        builder.Services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();

        if (seederType is null)
        {
            builder.Services.AddTransient<IMongoDbSeeder, MongoDbSeeder>();
        }
        else
        {
            builder.Services.AddTransient(typeof(IMongoDbSeeder), seederType);
        }

        builder.AddInitializer<IMongoDbInitializer>();
        if (registerConventions && !_conventionsRegistered)
        {
            // Use lock to ensure thread-safe registration
            lock (typeof(Extensions))
            {
                if (!_conventionsRegistered)
                {
                    RegisterConventions();
                }
            }
        }

        return builder;
    }

    private static void RegisterConventions()
    {
        // Register Decimal serializers - use try-catch to handle cases where serializer is already registered (e.g., in tests)
        try
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        }
        catch (BsonSerializationException)
        {
            // Serializer already registered, ignore
        }
        
        try
        {
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
        }
        catch (BsonSerializationException)
        {
            // Serializer already registered, ignore
        }
        
        // Register conventions - use try-catch to handle cases where conventions are already registered
        try
        {
            ConventionRegistry.Register("Mamey", new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
            }, _ => true);
        }
        catch (Exception)
        {
            // Conventions already registered, ignore (can throw various exceptions)
        }
        
        // Set flag to true only after attempting to register (whether successful or not)
        // This prevents multiple registration attempts
        _conventionsRegistered = true;
    }

    public static IMameyBuilder AddMongoRepository<TEntity, TIdentifiable>(this IMameyBuilder builder,
        string collectionName)
        where TEntity : IIdentifiable<TIdentifiable>
    {
        builder.Services.AddTransient<IMongoRepository<TEntity, TIdentifiable>>(sp =>
        {
            var database = sp.GetRequiredService<IMongoDatabase>();
            return new MongoRepository<TEntity, TIdentifiable>(database, collectionName);
        });

        return builder;
    }
}