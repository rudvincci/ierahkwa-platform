using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Accessibility.Domain.Repositories;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using Pupitre.Accessibility.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Accessibility.Infrastructure.Mongo.Services;
namespace Pupitre.Accessibility.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAccessProfileRepository, AccessProfileMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AccessProfileDocument, Guid>($"accessibility");
    }
}

