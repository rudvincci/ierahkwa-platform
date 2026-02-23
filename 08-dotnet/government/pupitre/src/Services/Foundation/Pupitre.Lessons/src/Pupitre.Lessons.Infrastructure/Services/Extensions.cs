using System;
using Pupitre.Lessons.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Lessons.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddLessonServices(this IServiceCollection services)
    {
        return services;
    }
}

