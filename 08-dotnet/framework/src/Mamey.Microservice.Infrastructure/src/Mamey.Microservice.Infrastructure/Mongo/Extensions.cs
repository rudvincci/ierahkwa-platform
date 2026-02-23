using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Mamey.Microservice.Infrastructure.Mongo.Factories;
using Mamey.Microservice.Infrastructure.Mongo.Repositories;
using Mamey.Microservice.Infrastructure.Mongo.Seeders;
using Mamey.Persistence.MongoDB;

namespace Mamey.Microservice.Infrastructure.Mongo
{
    internal static class Extensions
    {
        private static bool _conventionsRegistered;
        private const string SectionName = "mongo";
        
        public static IServiceCollection AddMongoRepository<TEntity, TIdentifiable>(this IServiceCollection services,
            string collectionName)
            where TEntity : IIdentifiable<TIdentifiable>
        {
            services.AddTransient<IMongoRepository<TEntity, TIdentifiable>>(sp =>
            {
                var database = sp.GetService<IMongoDatabase>();
                if (database is null)
                {
                    throw new InvalidOperationException("IMongoDatabase service is not registered.");
                }
                return new MongoRepository<TEntity, TIdentifiable>(database, collectionName);
            });

            return services;
        }

        internal static IServiceCollection AddMongo(this IServiceCollection services, string sectionName = SectionName,
            Type? seederType = null)
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                sectionName = SectionName;
            }

            // Try to get configuration without building service provider to avoid hangs
            // Configuration should be registered early in the pipeline
            IConfiguration? configuration = null;
            try
            {
                // Only build service provider if absolutely necessary and dispose immediately
                using (var tempProvider = services.BuildServiceProvider())
                {
                    configuration = tempProvider.GetService<IConfiguration>();
                }
            }
            catch
            {
                // If BuildServiceProvider fails, use default options
                configuration = null;
            }
            
            var mongoOptions = configuration?.GetOptions<MongoOptions>(sectionName) ?? new MongoOptions();
            services.AddSingleton(mongoOptions);
            services.AddSingleton<IMongoClient>(sp =>
            {
                var options = sp.GetService<MongoOptions>();
                if (options is null)
                {
                    throw new InvalidOperationException("MongoOptions service is not registered.");
                }
                return new MongoClient(options.ConnectionString);
            });
            services.AddTransient(sp =>
            {
                var options = sp.GetService<MongoOptions>();
                var client = sp.GetService<IMongoClient>();
                if (options is null || client is null)
                {
                    throw new InvalidOperationException("MongoOptions or IMongoClient service is not registered.");
                }
                return client.GetDatabase(options.Database);
            });
            services.AddTransient<IMongoSessionFactory, MongoSessionFactory>();

            if (seederType is null)
            {
                services.AddTransient<IMongoDbSeeder, MongoDbSeeder>();
            }
            else
            {
                services.AddTransient(typeof(IMongoDbSeeder), seederType);
            }

            if (!_conventionsRegistered)
            {
                RegisterConventions();
            }

            return services;
        }

        private static void RegisterConventions()
        {
            _conventionsRegistered = true;
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
            BsonSerializer.RegisterSerializer(typeof(decimal?),
                new NullableSerializer<decimal>(new DecimalSerializer(BsonType.Decimal128)));
            ConventionRegistry.Register("mamey", new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true),
                new EnumRepresentationConvention(BsonType.String),
            }, _ => true);
        }
        
    }
}