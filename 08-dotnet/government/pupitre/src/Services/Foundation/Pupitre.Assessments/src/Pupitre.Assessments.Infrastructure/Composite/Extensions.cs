using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Assessments.Domain.Repositories;

namespace Pupitre.Assessments.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAssessmentRepository, CompositeAssessmentRepository>();
        return builder;
    }
}
