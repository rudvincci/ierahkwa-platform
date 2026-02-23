using System;
using Pupitre.Fundraising.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Fundraising.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddCampaignServices(this IServiceCollection services)
    {
        return services;
    }
}

