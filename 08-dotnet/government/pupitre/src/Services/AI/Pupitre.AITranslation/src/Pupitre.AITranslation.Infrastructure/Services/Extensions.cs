using System;
using Pupitre.AITranslation.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITranslation.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddTranslationRequestServices(this IServiceCollection services)
    {
        return services;
    }
}

