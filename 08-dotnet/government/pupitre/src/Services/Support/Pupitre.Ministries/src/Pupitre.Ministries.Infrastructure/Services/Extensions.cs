using System;
using Pupitre.Ministries.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Ministries.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddMinistryDataServices(this IServiceCollection services)
    {
        return services;
    }
}

