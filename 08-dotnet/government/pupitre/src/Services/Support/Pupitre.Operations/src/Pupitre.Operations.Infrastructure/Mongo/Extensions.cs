using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Operations.Domain.Repositories;
using Pupitre.Operations.Infrastructure.Mongo.Documents;
using Pupitre.Operations.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Operations.Infrastructure.Mongo.Services;
namespace Pupitre.Operations.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IOperationMetricRepository, OperationMetricMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<OperationMetricDocument, Guid>($"operations");
    }
}

