using System;
using Mamey.Government.Identity.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Identity.Infrastructure.Clients
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

