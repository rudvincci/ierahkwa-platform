using System;
using Pupitre.AITranslation.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AITranslation.Infrastructure.Clients
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

