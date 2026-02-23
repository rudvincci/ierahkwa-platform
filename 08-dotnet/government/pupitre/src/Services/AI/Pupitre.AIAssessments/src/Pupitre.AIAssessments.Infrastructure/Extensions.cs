using Mamey;
﻿using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.AIAssessments.Application;
using Pupitre.AIAssessments.Infrastructure.Clients;
using Pupitre.AIAssessments.Infrastructure.EF;
using Pupitre.AIAssessments.Infrastructure.Exceptions;
using Pupitre.AIAssessments.Infrastructure.Metrics;
using Pupitre.AIAssessments.Infrastructure.Mongo;
using Pupitre.AIAssessments.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAssessments.Infrastructure.Redis;
using Pupitre.AIAssessments.Infrastructure.Composite;

[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Api")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.AIAssessments.Tests.Integration")]
namespace Pupitre.AIAssessments.Infrastructure
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
                .AddAIAssessmentServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseAIAssessmentPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

