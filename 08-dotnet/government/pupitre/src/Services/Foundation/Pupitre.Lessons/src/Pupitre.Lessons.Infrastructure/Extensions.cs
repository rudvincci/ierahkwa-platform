using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Lessons.Application;
using Pupitre.Lessons.Infrastructure.Clients;
using Pupitre.Lessons.Infrastructure.EF;
using Pupitre.Lessons.Infrastructure.Exceptions;
using Pupitre.Lessons.Infrastructure.Metrics;
using Pupitre.Lessons.Infrastructure.Mongo;
using Pupitre.Lessons.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Lessons.Infrastructure.Redis;
using Pupitre.Lessons.Infrastructure.Composite;
using Pupitre.Blockchain.Extensions;

[assembly: InternalsVisibleTo("Pupitre.Lessons.Api")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Lessons.Tests.Integration")]
namespace Pupitre.Lessons.Infrastructure
{
    internal static class Extensions
    {
        public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
        {
            builder.Services
                .AddInfrastructure()
                .AddPupitreBlockchain(builder.Configuration);

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
                .AddLessonServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseLessonPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

