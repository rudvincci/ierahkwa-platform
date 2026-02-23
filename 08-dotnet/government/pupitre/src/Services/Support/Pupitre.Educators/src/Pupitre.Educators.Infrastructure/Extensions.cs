using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Educators.Application;
using Pupitre.Educators.Infrastructure.Clients;
using Pupitre.Educators.Infrastructure.EF;
using Pupitre.Educators.Infrastructure.Exceptions;
using Pupitre.Educators.Infrastructure.Metrics;
using Pupitre.Educators.Infrastructure.Mongo;
using Pupitre.Educators.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Educators.Infrastructure.Redis;
using Pupitre.Educators.Infrastructure.Composite;
using Pupitre.Blockchain.Extensions;

[assembly: InternalsVisibleTo("Pupitre.Educators.Api")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Educators.Tests.Integration")]
namespace Pupitre.Educators.Infrastructure
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
                .AddEducatorServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseEducatorPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

