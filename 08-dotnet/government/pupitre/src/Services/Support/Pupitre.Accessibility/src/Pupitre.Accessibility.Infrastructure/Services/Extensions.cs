using System;
using Pupitre.Accessibility.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Accessibility.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAccessProfileServices(this IServiceCollection services)
    {
        return services;
    }
}

