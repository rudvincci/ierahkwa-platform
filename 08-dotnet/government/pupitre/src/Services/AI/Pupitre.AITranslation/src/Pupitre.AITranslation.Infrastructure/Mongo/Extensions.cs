using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AITranslation.Domain.Repositories;
using Pupitre.AITranslation.Infrastructure.Mongo.Documents;
using Pupitre.AITranslation.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AITranslation.Infrastructure.Mongo.Services;
namespace Pupitre.AITranslation.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ITranslationRequestRepository, TranslationRequestMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<TranslationRequestDocument, Guid>($"aitranslation");
    }
}

