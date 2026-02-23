using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.AIAssessments.Domain.Repositories;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using Pupitre.AIAssessments.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.AIAssessments.Infrastructure.Mongo.Services;
namespace Pupitre.AIAssessments.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAIAssessmentRepository, AIAssessmentMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<AIAssessmentDocument, Guid>($"aiassessments");
    }
}

