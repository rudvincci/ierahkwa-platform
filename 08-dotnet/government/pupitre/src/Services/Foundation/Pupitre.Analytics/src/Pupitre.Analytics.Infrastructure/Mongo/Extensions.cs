using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Analytics.Domain.Repositories;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using Pupitre.Analytics.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Analytics.Infrastructure.Mongo.Services;
namespace Pupitre.Analytics.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAnalyticRepository, AnalyticMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AnalyticDocument, Guid>($"analytics");
    }
}

