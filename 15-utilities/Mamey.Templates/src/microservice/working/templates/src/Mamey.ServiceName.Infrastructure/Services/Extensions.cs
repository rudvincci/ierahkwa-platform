using System;
using Mamey.ServiceName.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ServiceName.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddEntityNameServices(this IServiceCollection services)
    {
        return services;
    }
}

