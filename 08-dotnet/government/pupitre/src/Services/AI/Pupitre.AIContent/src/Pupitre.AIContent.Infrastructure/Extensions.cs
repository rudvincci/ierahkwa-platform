using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIContent.Application;
using Pupitre.AIContent.Infrastructure.Clients;
using Pupitre.AIContent.Infrastructure.EF;
using Pupitre.AIContent.Infrastructure.Exceptions;
using Pupitre.AIContent.Infrastructure.Metrics;
using Pupitre.AIContent.Infrastructure.Mongo;
using Pupitre.AIContent.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIContent.Infrastructure.Redis;
using Pupitre.AIContent.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIContent.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIContent.Tests.Integration")]
namespace Pupitre.AIContent.Infrastructure
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
                .AddContentGenerationServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseContentGenerationPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

