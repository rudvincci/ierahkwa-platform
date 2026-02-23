using System;
using Pupitre.AIBehavior.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIBehavior.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddBehaviorServices(this IServiceCollection services)
    {
        return services;
    }
}

