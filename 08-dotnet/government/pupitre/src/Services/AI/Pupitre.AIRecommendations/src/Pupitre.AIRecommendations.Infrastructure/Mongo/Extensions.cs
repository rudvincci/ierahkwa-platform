using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIRecommendations.Domain.Repositories;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Documents;
using Pupitre.AIRecommendations.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIRecommendations.Infrastructure.Mongo.Services;
namespace Pupitre.AIRecommendations.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAIRecommendationRepository, AIRecommendationMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AIRecommendationDocument, Guid>($"airecommendations");
    }
}

