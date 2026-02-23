using System;
using Pupitre.AIBehavior.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIBehavior.Infrastructure.Clients
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

