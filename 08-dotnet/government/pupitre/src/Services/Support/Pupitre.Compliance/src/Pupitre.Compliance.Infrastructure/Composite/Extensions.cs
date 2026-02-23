using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Compliance.Domain.Repositories;

namespace Pupitre.Compliance.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IComplianceRecordRepository, CompositeComplianceRecordRepository>();
        return builder;
    }
}
