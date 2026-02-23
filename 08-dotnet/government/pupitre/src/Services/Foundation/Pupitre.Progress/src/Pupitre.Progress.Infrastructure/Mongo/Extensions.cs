using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Progress.Domain.Repositories;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using Pupitre.Progress.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Progress.Infrastructure.Mongo.Services;
namespace Pupitre.Progress.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ILearningProgressRepository, LearningProgressMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<LearningProgressDocument, Guid>($"progress");
    }
}

