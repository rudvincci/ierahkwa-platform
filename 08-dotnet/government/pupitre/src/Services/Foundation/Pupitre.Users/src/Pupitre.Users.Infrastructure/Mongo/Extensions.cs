using Mamey;
using Mamey.Persistence.MongoDB;
using Pupitre.Users.Domain.Repositories;
using Pupitre.Users.Infrastructure.Mongo.Documents;
using Pupitre.Users.Infrastructure.Mongo.Repositories;
using Microsoft.Extensions.DependencyInjection;

using Pupitre.Users.Infrastructure.Mongo.Services;
namespace Pupitre.Users.Infrastructure.Mongo;

internal static class Extensions
{
    public static IMameyBuilder AddMongoDb(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, UserMongoRepository>();

        return builder
            .AddMongo()
            .AddMongoRepository<UserDocument, Guid>($"users");
    }
}

