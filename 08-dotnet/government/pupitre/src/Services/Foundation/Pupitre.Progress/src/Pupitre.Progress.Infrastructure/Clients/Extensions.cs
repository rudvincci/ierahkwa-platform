using System;
using Pupitre.Progress.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Progress.Infrastructure.Clients
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

