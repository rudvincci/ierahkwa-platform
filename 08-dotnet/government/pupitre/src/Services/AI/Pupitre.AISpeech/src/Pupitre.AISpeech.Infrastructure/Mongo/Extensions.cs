using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AISpeech.Domain.Repositories;
using Pupitre.AISpeech.Infrastructure.Mongo.Documents;
using Pupitre.AISpeech.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AISpeech.Infrastructure.Mongo.Services;
namespace Pupitre.AISpeech.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ISpeechRequestRepository, SpeechRequestMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<SpeechRequestDocument, Guid>($"aispeech");
    }
}

