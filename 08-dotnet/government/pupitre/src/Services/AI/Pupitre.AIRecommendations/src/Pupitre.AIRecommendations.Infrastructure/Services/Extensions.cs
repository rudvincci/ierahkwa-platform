using System;
using Pupitre.AIRecommendations.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIRecommendations.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAIRecommendationServices(this IServiceCollection services)
    {
        return services;
    }
}

