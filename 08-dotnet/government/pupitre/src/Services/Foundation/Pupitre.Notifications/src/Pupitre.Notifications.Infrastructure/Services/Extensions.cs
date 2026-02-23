using System;
using Pupitre.Notifications.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Notifications.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddNotificationServices(this IServiceCollection services)
    {
        return services;
    }
}

