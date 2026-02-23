using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.GLEs.Application;
using Pupitre.GLEs.Infrastructure.Clients;
using Pupitre.GLEs.Infrastructure.EF;
using Pupitre.GLEs.Infrastructure.Exceptions;
using Pupitre.GLEs.Infrastructure.Metrics;
using Pupitre.GLEs.Infrastructure.Mongo;
using Pupitre.GLEs.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.GLEs.Infrastructure.Redis;
using Pupitre.GLEs.Infrastructure.Composite;
using Pupitre.Blockchain.Extensions;

[assembly: InternalsVisibleTo("Pupitre.GLEs.Api")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.GLEs.Tests.Integration")]
namespace Pupitre.GLEs.Infrastructure
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
                .AddGLEServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseGLEPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

