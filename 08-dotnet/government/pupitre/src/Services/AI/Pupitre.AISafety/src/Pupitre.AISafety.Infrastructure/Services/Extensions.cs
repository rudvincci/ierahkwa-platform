using System;
using Pupitre.AISafety.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISafety.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddSafetyCheckServices(this IServiceCollection services)
    {
        return services;
    }
}

