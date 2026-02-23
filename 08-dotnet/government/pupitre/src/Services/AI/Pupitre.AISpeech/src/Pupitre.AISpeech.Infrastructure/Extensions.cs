using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AISpeech.Application;
using Pupitre.AISpeech.Infrastructure.Clients;
using Pupitre.AISpeech.Infrastructure.EF;
using Pupitre.AISpeech.Infrastructure.Exceptions;
using Pupitre.AISpeech.Infrastructure.Metrics;
using Pupitre.AISpeech.Infrastructure.Mongo;
using Pupitre.AISpeech.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISpeech.Infrastructure.Redis;
using Pupitre.AISpeech.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AISpeech.Api")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AISpeech.Tests.Integration")]
namespace Pupitre.AISpeech.Infrastructure
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
                .AddSpeechRequestServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseSpeechRequestPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

