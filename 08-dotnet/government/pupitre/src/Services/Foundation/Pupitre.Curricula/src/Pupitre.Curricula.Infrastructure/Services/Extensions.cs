using System;
using Pupitre.Curricula.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Curricula.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddCurriculumServices(this IServiceCollection services)
    {
        return services;
    }
}

