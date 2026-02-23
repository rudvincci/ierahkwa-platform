using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Rewards.Application;
using Pupitre.Rewards.Infrastructure.Clients;
using Pupitre.Rewards.Infrastructure.EF;
using Pupitre.Rewards.Infrastructure.Exceptions;
using Pupitre.Rewards.Infrastructure.Metrics;
using Pupitre.Rewards.Infrastructure.Mongo;
using Pupitre.Rewards.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Rewards.Infrastructure.Redis;
using Pupitre.Rewards.Infrastructure.Composite;
using Pupitre.Blockchain.Extensions;

[assembly: InternalsVisibleTo("Pupitre.Rewards.Api")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Rewards.Tests.Integration")]
namespace Pupitre.Rewards.Infrastructure
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
                .AddRewardServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseRewardPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

