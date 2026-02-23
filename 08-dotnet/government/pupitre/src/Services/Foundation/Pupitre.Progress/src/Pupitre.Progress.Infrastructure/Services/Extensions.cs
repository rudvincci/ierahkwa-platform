using System;
using Pupitre.Progress.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Progress.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddLearningProgressServices(this IServiceCollection services)
    {
        return services;
    }
}

