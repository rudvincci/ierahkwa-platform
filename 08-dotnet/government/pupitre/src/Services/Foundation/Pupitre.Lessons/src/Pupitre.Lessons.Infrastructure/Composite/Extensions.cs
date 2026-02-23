using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Lessons.Domain.Repositories;

namespace Pupitre.Lessons.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ILessonRepository, CompositeLessonRepository>();
        return builder;
    }
}
