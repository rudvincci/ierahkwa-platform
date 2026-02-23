using System;
using Pupitre.AIAssessments.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAssessments.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAIAssessmentServices(this IServiceCollection services)
    {
        return services;
    }
}

