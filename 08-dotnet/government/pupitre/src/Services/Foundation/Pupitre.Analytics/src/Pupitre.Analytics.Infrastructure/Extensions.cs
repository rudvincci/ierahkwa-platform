using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Analytics.Application;
using Pupitre.Analytics.Infrastructure.Clients;
using Pupitre.Analytics.Infrastructure.EF;
using Pupitre.Analytics.Infrastructure.Exceptions;
using Pupitre.Analytics.Infrastructure.Metrics;
using Pupitre.Analytics.Infrastructure.Mongo;
using Pupitre.Analytics.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Analytics.Infrastructure.Redis;
using Pupitre.Analytics.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Analytics.Api")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Analytics.Tests.Integration")]
namespace Pupitre.Analytics.Infrastructure
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
                .AddAnalyticServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAnalyticPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

