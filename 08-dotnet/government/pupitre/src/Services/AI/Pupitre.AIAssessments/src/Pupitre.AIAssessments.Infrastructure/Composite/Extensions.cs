using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAssessments.Domain.Repositories;

namespace Pupitre.AIAssessments.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAIAssessmentRepository, CompositeAIAssessmentRepository>();
        return builder;
    }
}
