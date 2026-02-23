using System;
using Mamey.ServiceName.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ServiceName.Infrastructure.Clients
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

