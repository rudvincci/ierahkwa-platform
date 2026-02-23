using System;
using Pupitre.Aftercare.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Aftercare.Infrastructure.Clients
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

