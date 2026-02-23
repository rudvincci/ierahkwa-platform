using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIAdaptive.Application;
using Pupitre.AIAdaptive.Infrastructure.Clients;
using Pupitre.AIAdaptive.Infrastructure.EF;
using Pupitre.AIAdaptive.Infrastructure.Exceptions;
using Pupitre.AIAdaptive.Infrastructure.Metrics;
using Pupitre.AIAdaptive.Infrastructure.Mongo;
using Pupitre.AIAdaptive.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAdaptive.Infrastructure.Redis;
using Pupitre.AIAdaptive.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIAdaptive.Tests.Integration")]
namespace Pupitre.AIAdaptive.Infrastructure
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
                .AddAdaptiveLearningServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAdaptiveLearningPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

