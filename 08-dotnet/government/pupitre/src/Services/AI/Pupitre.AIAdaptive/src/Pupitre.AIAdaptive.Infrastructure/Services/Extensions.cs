using System;
using Pupitre.AIAdaptive.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAdaptive.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAdaptiveLearningServices(this IServiceCollection services)
    {
        return services;
    }
}

