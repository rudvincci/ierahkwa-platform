using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Users.Domain.Repositories;

namespace Pupitre.Users.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IUserRepository, CompositeUserRepository>();
        return builder;
    }
}
