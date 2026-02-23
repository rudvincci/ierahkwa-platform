using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIVision.Application;
using Pupitre.AIVision.Infrastructure.Clients;
using Pupitre.AIVision.Infrastructure.EF;
using Pupitre.AIVision.Infrastructure.Exceptions;
using Pupitre.AIVision.Infrastructure.Metrics;
using Pupitre.AIVision.Infrastructure.Mongo;
using Pupitre.AIVision.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIVision.Infrastructure.Redis;
using Pupitre.AIVision.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIVision.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIVision.Tests.Integration")]
namespace Pupitre.AIVision.Infrastructure
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
                .AddVisionAnalysisServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseVisionAnalysisPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

