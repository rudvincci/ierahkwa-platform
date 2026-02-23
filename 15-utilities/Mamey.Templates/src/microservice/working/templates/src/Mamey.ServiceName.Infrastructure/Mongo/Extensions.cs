using Mamey.Persistence.MongoDB;
using Mamey.ServiceName.Domain.Repositories;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using Mamey.ServiceName.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ServiceName.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IEntityNameRepository, EntityNameMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<EntityNameDocument, Guid>($"servicename");
    }
}

