using System;
using Pupitre.Compliance.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Compliance.Infrastructure.Clients
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

