using System;
using Pupitre.AIAdaptive.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.AIAdaptive.Infrastructure.Clients
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

