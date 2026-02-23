using System;
using Pupitre.Parents.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Parents.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddParentServices(this IServiceCollection services)
    {
        return services;
    }
}

