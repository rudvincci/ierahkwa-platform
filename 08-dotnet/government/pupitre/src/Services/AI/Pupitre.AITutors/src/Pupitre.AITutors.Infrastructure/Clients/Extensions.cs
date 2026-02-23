using System;
using Pupitre.AITutors.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITutors.Infrastructure.Clients
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

