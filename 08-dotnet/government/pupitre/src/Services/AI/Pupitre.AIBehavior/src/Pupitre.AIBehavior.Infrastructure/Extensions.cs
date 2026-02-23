using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIBehavior.Application;
using Pupitre.AIBehavior.Infrastructure.Clients;
using Pupitre.AIBehavior.Infrastructure.EF;
using Pupitre.AIBehavior.Infrastructure.Exceptions;
using Pupitre.AIBehavior.Infrastructure.Metrics;
using Pupitre.AIBehavior.Infrastructure.Mongo;
using Pupitre.AIBehavior.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIBehavior.Infrastructure.Redis;
using Pupitre.AIBehavior.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIBehavior.Tests.Integration")]
namespace Pupitre.AIBehavior.Infrastructure
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
                .AddBehaviorServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseBehaviorPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

