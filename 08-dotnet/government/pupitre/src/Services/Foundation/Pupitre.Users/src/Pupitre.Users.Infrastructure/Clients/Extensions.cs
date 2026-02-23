using System;
using Pupitre.Users.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Users.Infrastructure.Clients
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

