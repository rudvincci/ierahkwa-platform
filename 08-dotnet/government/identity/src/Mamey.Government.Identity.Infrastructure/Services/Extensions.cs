using System;
using Mamey.CQRS.Events;
using Mamey.Government.Identity.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddSingleton<IEventMapper, EventMapper>();
        return services;
    }
}

