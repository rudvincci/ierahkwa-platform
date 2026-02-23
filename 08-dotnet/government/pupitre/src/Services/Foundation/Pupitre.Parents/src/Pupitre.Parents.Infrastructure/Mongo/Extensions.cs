using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Parents.Domain.Repositories;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using Pupitre.Parents.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Parents.Infrastructure.Mongo.Services;
namespace Pupitre.Parents.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IParentRepository, ParentMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<ParentDocument, Guid>($"parents");
    }
}

