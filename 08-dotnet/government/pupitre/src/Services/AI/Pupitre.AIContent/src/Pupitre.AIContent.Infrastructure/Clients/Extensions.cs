using System;
using Pupitre.AIContent.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIContent.Infrastructure.Clients
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

