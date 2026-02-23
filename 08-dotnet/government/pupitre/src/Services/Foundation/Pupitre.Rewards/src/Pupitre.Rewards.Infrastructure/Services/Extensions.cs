using System;
using Pupitre.Rewards.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Rewards.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddRewardServices(this IServiceCollection services)
    {
        return services;
    }
}

