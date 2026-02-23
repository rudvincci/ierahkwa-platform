using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Rewards.Domain.Repositories;
using Pupitre.Rewards.Infrastructure.Mongo.Documents;
using Pupitre.Rewards.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Rewards.Infrastructure.Mongo.Services;
namespace Pupitre.Rewards.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IRewardRepository, RewardMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<RewardDocument, Guid>($"rewards");
    }
}

