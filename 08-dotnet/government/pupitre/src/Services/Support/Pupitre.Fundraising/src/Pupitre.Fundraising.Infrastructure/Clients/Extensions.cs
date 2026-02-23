using System;
using Pupitre.Fundraising.Application.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.Fundraising.Infrastructure.Clients
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

