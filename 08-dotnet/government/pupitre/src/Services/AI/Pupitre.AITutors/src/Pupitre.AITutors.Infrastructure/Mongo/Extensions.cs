using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AITutors.Domain.Repositories;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using Pupitre.AITutors.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AITutors.Infrastructure.Mongo.Services;
namespace Pupitre.AITutors.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ITutorRepository, TutorMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<TutorDocument, Guid>($"aitutors");
    }
}

