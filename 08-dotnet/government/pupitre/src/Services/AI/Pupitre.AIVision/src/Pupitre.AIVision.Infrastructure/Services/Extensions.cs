using System;
using Pupitre.AIVision.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIVision.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddVisionAnalysisServices(this IServiceCollection services)
    {
        return services;
    }
}

