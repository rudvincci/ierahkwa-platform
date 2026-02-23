using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Fundraising.Domain.Repositories;

namespace Pupitre.Fundraising.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ICampaignRepository, CompositeCampaignRepository>();
        return builder;
    }
}
