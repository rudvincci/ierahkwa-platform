using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITranslation.Domain.Repositories;

namespace Pupitre.AITranslation.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ITranslationRequestRepository, CompositeTranslationRequestRepository>();
        return builder;
    }
}
