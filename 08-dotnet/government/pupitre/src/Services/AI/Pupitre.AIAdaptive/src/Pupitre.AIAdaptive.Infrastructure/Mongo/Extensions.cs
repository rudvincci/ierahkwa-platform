using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIAdaptive.Domain.Repositories;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIAdaptive.Infrastructure.Mongo.Services;
namespace Pupitre.AIAdaptive.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAdaptiveLearningRepository, AdaptiveLearningMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AdaptiveLearningDocument, Guid>($"aiadaptive");
    }
}

