using System;
using Pupitre.Analytics.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Analytics.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAnalyticServices(this IServiceCollection services)
    {
        return services;
    }
}

