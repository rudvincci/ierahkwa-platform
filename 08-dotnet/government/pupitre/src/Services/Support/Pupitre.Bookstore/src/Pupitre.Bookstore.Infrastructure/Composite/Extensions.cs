using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Bookstore.Domain.Repositories;

namespace Pupitre.Bookstore.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IBookRepository, CompositeBookRepository>();
        return builder;
    }
}
