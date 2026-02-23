using System;
using Pupitre.Assessments.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Assessments.Infrastructure.Clients
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

