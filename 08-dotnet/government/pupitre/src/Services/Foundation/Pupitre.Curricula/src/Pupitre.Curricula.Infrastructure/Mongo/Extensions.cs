using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Curricula.Domain.Repositories;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using Pupitre.Curricula.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Curricula.Infrastructure.Mongo.Services;
namespace Pupitre.Curricula.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ICurriculumRepository, CurriculumMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<CurriculumDocument, Guid>($"curricula");
    }
}

