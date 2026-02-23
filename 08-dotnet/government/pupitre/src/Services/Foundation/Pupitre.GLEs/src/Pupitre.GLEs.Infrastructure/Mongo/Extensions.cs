using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.GLEs.Domain.Repositories;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using Pupitre.GLEs.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.GLEs.Infrastructure.Mongo.Services;
namespace Pupitre.GLEs.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IGLERepository, GLEMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<GLEDocument, Guid>($"gles");
    }
}

