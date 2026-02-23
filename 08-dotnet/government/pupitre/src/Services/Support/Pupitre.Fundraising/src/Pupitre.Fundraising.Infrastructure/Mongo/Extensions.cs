using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Fundraising.Domain.Repositories;
using Pupitre.Fundraising.Infrastructure.Mongo.Documents;
using Pupitre.Fundraising.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Fundraising.Infrastructure.Mongo.Services;
namespace Pupitre.Fundraising.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ICampaignRepository, CampaignMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<CampaignDocument, Guid>($"fundraising");
    }
}

