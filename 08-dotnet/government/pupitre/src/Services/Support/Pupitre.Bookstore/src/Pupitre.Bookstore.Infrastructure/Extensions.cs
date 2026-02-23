using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Bookstore.Application;
using Pupitre.Bookstore.Infrastructure.Clients;
using Pupitre.Bookstore.Infrastructure.EF;
using Pupitre.Bookstore.Infrastructure.Exceptions;
using Pupitre.Bookstore.Infrastructure.Metrics;
using Pupitre.Bookstore.Infrastructure.Mongo;
using Pupitre.Bookstore.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Bookstore.Infrastructure.Redis;
using Pupitre.Bookstore.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Bookstore.Api")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Bookstore.Tests.Integration")]
namespace Pupitre.Bookstore.Infrastructure
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
                .AddBookServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseBookPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

