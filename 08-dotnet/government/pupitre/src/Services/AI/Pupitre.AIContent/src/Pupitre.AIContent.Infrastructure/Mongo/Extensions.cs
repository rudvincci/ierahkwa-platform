using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIContent.Domain.Repositories;
using Pupitre.AIContent.Infrastructure.Mongo.Documents;
using Pupitre.AIContent.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIContent.Infrastructure.Mongo.Services;
namespace Pupitre.AIContent.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IContentGenerationRepository, ContentGenerationMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<ContentGenerationDocument, Guid>($"aicontent");
    }
}

