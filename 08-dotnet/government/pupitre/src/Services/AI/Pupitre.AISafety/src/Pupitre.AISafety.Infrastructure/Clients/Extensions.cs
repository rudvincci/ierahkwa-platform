using System;
using Pupitre.AISafety.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AISafety.Infrastructure.Clients
{
    internal static class Extensions
    {
        public static IServiceCollection AddServiceClients(this IServiceCollection services)
        {
            services.AddScoped<ISamplesServiceClient, SamplesServiceClient>();
            return services;
        }
    }
}

