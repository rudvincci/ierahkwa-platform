using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Assessments.Domain.Repositories;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using Pupitre.Assessments.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Assessments.Infrastructure.Mongo.Services;
namespace Pupitre.Assessments.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAssessmentRepository, AssessmentMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AssessmentDocument, Guid>($"assessments");
    }
}

