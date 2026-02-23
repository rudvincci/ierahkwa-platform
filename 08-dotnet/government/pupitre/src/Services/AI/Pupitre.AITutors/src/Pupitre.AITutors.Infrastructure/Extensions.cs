using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AITutors.Application;
using Pupitre.AITutors.Infrastructure.Clients;
using Pupitre.AITutors.Infrastructure.EF;
using Pupitre.AITutors.Infrastructure.Exceptions;
using Pupitre.AITutors.Infrastructure.Metrics;
using Pupitre.AITutors.Infrastructure.Mongo;
using Pupitre.AITutors.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITutors.Infrastructure.Redis;
using Pupitre.AITutors.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AITutors.Api")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AITutors.Tests.Integration")]
namespace Pupitre.AITutors.Infrastructure
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
                .AddTutorServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseTutorPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

