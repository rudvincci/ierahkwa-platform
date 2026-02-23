using System;
using Pupitre.Assessments.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Assessments.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddAssessmentServices(this IServiceCollection services)
    {
        return services;
    }
}

