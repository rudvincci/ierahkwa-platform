using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Aftercare.Domain.Repositories;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using Pupitre.Aftercare.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Aftercare.Infrastructure.Mongo.Services;
namespace Pupitre.Aftercare.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAftercarePlanRepository, AftercarePlanMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AftercarePlanDocument, Guid>($"aftercare");
    }
}

