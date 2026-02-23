using System;
using Pupitre.Curricula.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Curricula.Infrastructure.Clients
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

