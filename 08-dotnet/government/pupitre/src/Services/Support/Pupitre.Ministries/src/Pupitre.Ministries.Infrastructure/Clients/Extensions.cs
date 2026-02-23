using System;
using Pupitre.Ministries.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Ministries.Infrastructure.Clients
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

