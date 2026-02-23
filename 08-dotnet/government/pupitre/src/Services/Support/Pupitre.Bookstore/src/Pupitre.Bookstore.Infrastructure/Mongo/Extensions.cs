using Mamey;
﻿using Mamey.Persistence.MongoDB;
using Pupitre.Bookstore.Domain.Repositories;
using Pupitre.Bookstore.Infrastructure.Mongo.Documents;
using Pupitre.Bookstore.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Bookstore.Infrastructure.Mongo.Services;
namespace Pupitre.Bookstore.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IBookRepository, BookMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<BookDocument, Guid>($"bookstore");
    }
}

