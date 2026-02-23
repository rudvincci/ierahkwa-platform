using System;
using Pupitre.Compliance.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Compliance.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddComplianceRecordServices(this IServiceCollection services)
    {
        return services;
    }
}

