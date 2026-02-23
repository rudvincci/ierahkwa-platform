using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Accessibility.Application;
using Pupitre.Accessibility.Infrastructure.Clients;
using Pupitre.Accessibility.Infrastructure.EF;
using Pupitre.Accessibility.Infrastructure.Exceptions;
using Pupitre.Accessibility.Infrastructure.Metrics;
using Pupitre.Accessibility.Infrastructure.Mongo;
using Pupitre.Accessibility.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Accessibility.Infrastructure.Redis;
using Pupitre.Accessibility.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Accessibility.Api")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Accessibility.Tests.Integration")]
namespace Pupitre.Accessibility.Infrastructure
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
                .AddAccessProfileServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAccessProfilePostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

