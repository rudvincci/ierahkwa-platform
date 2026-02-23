using System;
using Pupitre.AIContent.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIContent.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddContentGenerationServices(this IServiceCollection services)
    {
        return services;
    }
}

