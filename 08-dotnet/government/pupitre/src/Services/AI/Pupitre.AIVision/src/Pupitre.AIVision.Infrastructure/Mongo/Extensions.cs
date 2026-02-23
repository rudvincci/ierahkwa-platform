using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIVision.Domain.Repositories;
using Pupitre.AIVision.Infrastructure.Mongo.Documents;
using Pupitre.AIVision.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIVision.Infrastructure.Mongo.Services;
namespace Pupitre.AIVision.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IVisionAnalysisRepository, VisionAnalysisMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<VisionAnalysisDocument, Guid>($"aivision");
    }
}

