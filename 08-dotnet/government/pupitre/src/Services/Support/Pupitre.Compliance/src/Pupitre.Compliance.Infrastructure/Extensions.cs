using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Compliance.Application;
using Pupitre.Compliance.Infrastructure.Clients;
using Pupitre.Compliance.Infrastructure.EF;
using Pupitre.Compliance.Infrastructure.Exceptions;
using Pupitre.Compliance.Infrastructure.Metrics;
using Pupitre.Compliance.Infrastructure.Mongo;
using Pupitre.Compliance.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Compliance.Infrastructure.Redis;
using Pupitre.Compliance.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Compliance.Api")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Compliance.Tests.Integration")]
namespace Pupitre.Compliance.Infrastructure
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
                .AddComplianceRecordServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseComplianceRecordPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

