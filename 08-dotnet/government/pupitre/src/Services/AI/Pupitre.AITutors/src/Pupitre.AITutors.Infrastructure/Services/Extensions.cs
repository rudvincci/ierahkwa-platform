using System;
using Pupitre.AITutors.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITutors.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddTutorServices(this IServiceCollection services)
    {
        return services;
    }
}

