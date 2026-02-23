using System.Runtime.CompilerServices;
using Mamey;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Pupitre.Users.Application;
using Pupitre.Users.Infrastructure.Clients;
using Pupitre.Users.Infrastructure.EF;
using Pupitre.Users.Infrastructure.Exceptions;
using Pupitre.Users.Infrastructure.Metrics;
using Pupitre.Users.Infrastructure.Mongo;
using Pupitre.Users.Infrastructure.Services;
using Mamey.Microservice.Infrastructure.Logging;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Users.Infrastructure.Redis;
using Pupitre.Users.Infrastructure.Composite;
using Pupitre.Blockchain.Extensions;

[assembly: InternalsVisibleTo("Pupitre.Users.Api")]
[assembly: InternalsVisibleTo("Pupitre.Users.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Pupitre.Users.Tests.Integration")]
namespace Pupitre.Users.Infrastructure
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
                .AddUserServices();
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
        {
            builder
                .UseSharedInfrastructure()
                .UseUserPostgres()
                .UseRabbitMq()
                .AddSubscriptions();
            return builder;
        }

        
    }
}

