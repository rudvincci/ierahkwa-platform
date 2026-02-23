using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Compliance.Domain.Repositories;
using Pupitre.Compliance.Infrastructure.Mongo.Documents;
using Pupitre.Compliance.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Compliance.Infrastructure.Mongo.Services;
namespace Pupitre.Compliance.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<ComplianceRecordDocument, Guid>($"compliance");
    }
}

