using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIBehavior.Domain.Repositories;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using Pupitre.AIBehavior.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIBehavior.Infrastructure.Mongo.Services;
namespace Pupitre.AIBehavior.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IBehaviorRepository, BehaviorMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<BehaviorDocument, Guid>($"aibehavior");
    }
}

