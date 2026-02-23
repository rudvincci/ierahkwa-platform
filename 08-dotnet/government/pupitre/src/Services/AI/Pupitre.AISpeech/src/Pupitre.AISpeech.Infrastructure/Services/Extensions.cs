using System;
using Pupitre.AISpeech.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISpeech.Infrastructure.Services;

internal static class Extensions
{
    public static IServiceCollection AddSpeechRequestServices(this IServiceCollection services)
    {
        return services;
    }
}

