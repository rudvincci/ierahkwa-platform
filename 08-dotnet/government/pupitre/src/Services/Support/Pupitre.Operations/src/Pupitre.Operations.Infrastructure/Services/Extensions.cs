using System;
using Pupitre.Operations.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Operations.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddOperationMetricServices(this IServiceCollection services)
    {
        return services;
    }
}

