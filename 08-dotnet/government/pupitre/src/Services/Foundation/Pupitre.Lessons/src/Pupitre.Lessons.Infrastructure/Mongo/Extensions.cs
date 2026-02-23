using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Lessons.Domain.Repositories;
using Pupitre.Lessons.Infrastructure.Mongo.Documents;
using Pupitre.Lessons.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Lessons.Infrastructure.Mongo.Services;
namespace Pupitre.Lessons.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ILessonRepository, LessonMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<LessonDocument, Guid>($"lessons");
    }
}

