using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AISafety.Domain.Repositories;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using Pupitre.AISafety.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AISafety.Infrastructure.Mongo.Services;
namespace Pupitre.AISafety.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ISafetyCheckRepository, SafetyCheckMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<SafetyCheckDocument, Guid>($"aisafety");
    }
}

