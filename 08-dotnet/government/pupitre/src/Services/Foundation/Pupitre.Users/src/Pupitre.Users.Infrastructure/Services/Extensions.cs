using System;
using Pupitre.Users.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Users.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        return services;
    }
}

