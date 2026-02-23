using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIRecommendations.Application;
using Pupitre.AIRecommendations.Infrastructure.Clients;
using Pupitre.AIRecommendations.Infrastructure.EF;
using Pupitre.AIRecommendations.Infrastructure.Exceptions;
using Pupitre.AIRecommendations.Infrastructure.Metrics;
using Pupitre.AIRecommendations.Infrastructure.Mongo;
using Pupitre.AIRecommendations.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIRecommendations.Infrastructure.Redis;
using Pupitre.AIRecommendations.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIRecommendations.Tests.Integration")]
namespace Pupitre.AIRecommendations.Infrastructure
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
                .AddAIRecommendationServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAIRecommendationPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

