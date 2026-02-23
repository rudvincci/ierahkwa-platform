using System;
using Pupitre.Bookstore.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Bookstore.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddBookServices(this IServiceCollection services)
    {
        return services;
    }
}

