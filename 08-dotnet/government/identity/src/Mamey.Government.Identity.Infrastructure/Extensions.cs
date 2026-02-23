using System.Linq;
using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Mamey.Government.Identity.Application;
using Mamey.Government.Identity.Application.Auth;
using Mamey.Government.Identity.Application.Services;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Infrastructure.Auth;
using Mamey.Time;
using Mamey.Microservice.Infrastructure.Time;
using Mamey.Government.Identity.Infrastructure.Clients;
using Mamey.Government.Identity.Infrastructure.EF;
using Mamey.Government.Identity.Infrastructure.Exceptions;
using Mamey.Government.Identity.Infrastructure.Metrics;
using Mamey.Government.Identity.Infrastructure.Mongo;
using Mamey.Government.Identity.Infrastructure.Redis;
using Mamey.Government.Identity.Infrastructure.Composite;
using Mamey.Government.Identity.Infrastructure.Logging;
using Mamey.Government.Identity.Infrastructure.Services;
using Mamey.Security;
using Mamey.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Api")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration")]
namespace Mamey.Government.Identity.Infrastructure
{
    internal static class Extensions
    {
        public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
        {
            // ALWAYS ensure logging is registered FIRST before any handlers are scanned
            // This is critical - ILogger<T> must be available when handlers are registered
            // AddLogging() is idempotent, so calling it multiple times is safe
            // This guarantees ILoggerFactory is in the service collection when AddCommandHandlers() validates dependencies
            builder.Services.AddLogging();
            
            builder.Services.AddInfrastructure();
            
            // Register application services BEFORE AddMicroserviceSharedInfrastructure
            // because AddMicroserviceSharedInfrastructure calls AddQueryHandlers() which validates dependencies
            builder.Services.AddApplicationServices();
            
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
            services.AddSingleton<IClock, UtcClock>();
            services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHostedService<MetricsJob>();
            services.AddScoped<IJwtProvider, JwtProvider>();
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

