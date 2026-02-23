using System;
using Pupitre.Operations.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Operations.Infrastructure.Clients
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

