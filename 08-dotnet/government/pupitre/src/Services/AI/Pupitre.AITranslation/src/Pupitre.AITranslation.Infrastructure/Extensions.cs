using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AITranslation.Application;
using Pupitre.AITranslation.Infrastructure.Clients;
using Pupitre.AITranslation.Infrastructure.EF;
using Pupitre.AITranslation.Infrastructure.Exceptions;
using Pupitre.AITranslation.Infrastructure.Metrics;
using Pupitre.AITranslation.Infrastructure.Mongo;
using Pupitre.AITranslation.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITranslation.Infrastructure.Redis;
using Pupitre.AITranslation.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AITranslation.Api")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AITranslation.Tests.Integration")]
namespace Pupitre.AITranslation.Infrastructure
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
                .AddTranslationRequestServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseTranslationRequestPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

