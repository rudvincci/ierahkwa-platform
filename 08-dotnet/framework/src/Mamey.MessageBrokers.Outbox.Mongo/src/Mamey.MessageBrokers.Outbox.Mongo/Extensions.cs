using Mamey.MessageBrokers.Outbox.Messages;
using Mamey.MessageBrokers.Outbox.Mongo.Internals;
using Mamey.Persistence.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;

namespace Mamey.MessageBrokers.Outbox.Mongo;

public static class Extensions
{
    public static IMessageOutboxConfigurator AddMongo(this IMessageOutboxConfigurator configurator)
    {
        var builder = configurator.Builder;
        var options = configurator.Options;

        var inboxCollection = string.IsNullOrWhiteSpace(options.InboxCollection)
            ? "inbox"
            : options.InboxCollection;
        var outboxCollection = string.IsNullOrWhiteSpace(options.OutboxCollection)
            ? "outbox"
            : options.OutboxCollection;

        builder.AddMongoRepository<InboxMessage, string>(inboxCollection);
        builder.AddMongoRepository<OutboxMessage, string>(outboxCollection);
        builder.AddInitializer<MongoOutboxInitializer>();
        builder.Services.AddTransient<IMessageOutbox, MongoMessageOutbox>();
        builder.Services.AddTransient<IMessageOutboxAccessor, MongoMessageOutbox>();
        builder.Services.AddTransient<MongoOutboxInitializer>();

        // Register OutboxMessage class map - use try-catch to handle cases where class map is already registered (e.g., in tests)
        try
        {
            BsonClassMap.RegisterClassMap<OutboxMessage>(m =>
            {
                m.AutoMap();
                m.UnmapMember(p => p.Message);
                m.UnmapMember(p => p.MessageContext);
            });
        }
        catch (ArgumentException)
        {
            // Class map already registered, ignore
        }

        return configurator;
    }
}