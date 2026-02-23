using System;
using Pupitre.Rewards.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Rewards.Infrastructure.Clients
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

