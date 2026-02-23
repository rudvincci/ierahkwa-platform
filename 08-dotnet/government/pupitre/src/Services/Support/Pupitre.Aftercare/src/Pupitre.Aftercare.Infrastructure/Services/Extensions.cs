using System;
using Pupitre.Aftercare.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Aftercare.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAftercarePlanServices(this IServiceCollection services)
    {
        return services;
    }
}

