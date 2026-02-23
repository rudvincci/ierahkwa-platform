using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Educators.Domain.Repositories;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using Pupitre.Educators.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Educators.Infrastructure.Mongo.Services;
namespace Pupitre.Educators.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IEducatorRepository, EducatorMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<EducatorDocument, Guid>($"educators");
    }
}

