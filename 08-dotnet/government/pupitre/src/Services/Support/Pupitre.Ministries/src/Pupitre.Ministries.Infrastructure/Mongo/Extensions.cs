using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Ministries.Domain.Repositories;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using Pupitre.Ministries.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Ministries.Infrastructure.Mongo.Services;
namespace Pupitre.Ministries.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IMinistryDataRepository, MinistryDataMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<MinistryDataDocument, Guid>($"ministries");
    }
}

