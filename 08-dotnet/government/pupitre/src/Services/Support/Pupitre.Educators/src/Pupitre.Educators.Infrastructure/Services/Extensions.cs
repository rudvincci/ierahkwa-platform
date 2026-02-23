using System;
using Pupitre.Educators.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Educators.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddEducatorServices(this IServiceCollection services)
    {
        return services;
    }
}

