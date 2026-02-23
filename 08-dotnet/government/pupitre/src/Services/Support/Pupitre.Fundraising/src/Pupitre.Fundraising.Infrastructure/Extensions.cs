using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Fundraising.Application;
using Pupitre.Fundraising.Infrastructure.Clients;
using Pupitre.Fundraising.Infrastructure.EF;
using Pupitre.Fundraising.Infrastructure.Exceptions;
using Pupitre.Fundraising.Infrastructure.Metrics;
using Pupitre.Fundraising.Infrastructure.Mongo;
using Pupitre.Fundraising.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Fundraising.Infrastructure.Redis;
using Pupitre.Fundraising.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Fundraising.Api")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Fundraising.Tests.Integration")]
namespace Pupitre.Fundraising.Infrastructure
{
    internal static class Extensions
    {
        public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
        {
            builder.Services.AddInfrastructure();

            return builder
                .AddErrorHandler<ExceptionToResponseMapper>()
                .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
                .AddMongoDb()
                .AddPostgresDb()
                .AddRedisRepositories()
                .AddCompositeRepositories()
                .AddHandlersLogging()
                .AddMicroserviceSharedInfrastructure()
                .AddApplication()
                ;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHostedService<MetricsJob>();
            services.AddSingleton<IEventMapper, EventMapper>();
            services.AddSingleton<CustomMetricsMiddleware>();
            services
                .AddServiceClients()
                .AddCampaignServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseCampaignPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

