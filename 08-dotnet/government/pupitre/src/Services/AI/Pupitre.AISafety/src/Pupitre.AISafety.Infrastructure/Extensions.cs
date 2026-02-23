using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AISafety.Application;
using Pupitre.AISafety.Infrastructure.Clients;
using Pupitre.AISafety.Infrastructure.EF;
using Pupitre.AISafety.Infrastructure.Exceptions;
using Pupitre.AISafety.Infrastructure.Metrics;
using Pupitre.AISafety.Infrastructure.Mongo;
using Pupitre.AISafety.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISafety.Infrastructure.Redis;
using Pupitre.AISafety.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AISafety.Api")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AISafety.Tests.Integration")]
namespace Pupitre.AISafety.Infrastructure
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
                .AddSafetyCheckServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseSafetyCheckPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

