using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Aftercare.Application;
using Pupitre.Aftercare.Infrastructure.Clients;
using Pupitre.Aftercare.Infrastructure.EF;
using Pupitre.Aftercare.Infrastructure.Exceptions;
using Pupitre.Aftercare.Infrastructure.Metrics;
using Pupitre.Aftercare.Infrastructure.Mongo;
using Pupitre.Aftercare.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Aftercare.Infrastructure.Redis;
using Pupitre.Aftercare.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.Aftercare.Api")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Aftercare.Tests.Integration")]
namespace Pupitre.Aftercare.Infrastructure
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
                .AddAftercarePlanServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAftercarePlanPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

