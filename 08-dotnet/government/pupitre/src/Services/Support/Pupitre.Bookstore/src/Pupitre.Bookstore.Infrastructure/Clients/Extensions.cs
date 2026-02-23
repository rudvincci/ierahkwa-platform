using System;
using Pupitre.Bookstore.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Bookstore.Infrastructure.Clients
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

