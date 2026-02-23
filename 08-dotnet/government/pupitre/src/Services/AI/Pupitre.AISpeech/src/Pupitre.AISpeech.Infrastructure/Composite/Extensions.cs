using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISpeech.Domain.Repositories;

namespace Pupitre.AISpeech.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ISpeechRequestRepository, CompositeSpeechRequestRepository>();
        return builder;
    }
}
