using System;
using Pupitre.GLEs.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.GLEs.Infrastructure.Clients
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

