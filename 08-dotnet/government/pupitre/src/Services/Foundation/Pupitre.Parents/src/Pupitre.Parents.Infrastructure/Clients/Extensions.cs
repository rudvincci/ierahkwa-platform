using System;
using Pupitre.Parents.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Parents.Infrastructure.Clients
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

