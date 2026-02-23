using System;
using Pupitre.Lessons.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Lessons.Infrastructure.Clients
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

