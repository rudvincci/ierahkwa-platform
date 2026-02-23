using System.Runtime.CompilerServices;
using Mamey.Emails;
using Mamey.FWID.Notifications.Application;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Infrastructure.Clients;
using Mamey.FWID.Notifications.Infrastructure.Exceptions;
using Mamey.FWID.Notifications.Infrastructure.Grpc.Interceptors;
using Mamey.FWID.Notifications.Infrastructure.MinIO;
using Mamey.FWID.Notifications.Infrastructure.Services;
using Mamey.Microservice.Infrastructure;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.Persistence.Minio;
using Mamey.Security;
using Mamey.WebApi;
using Mamey.WebApi.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Mamey.FWID.Notifications")]
[assembly: InternalsVisibleTo("Mamey.FWID.Notifications.Api")]
namespace Mamey.FWID.Notifications.Infrastructure;

internal static class Extensions
{
    public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
    {
        // Ensure logging is registered before handlers are scanned
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        builder.Services.AddInfrastructure();

        // Register application services BEFORE AddMicroserviceSharedInfrastructure
        // CRITICAL: AddMicroserviceSharedInfrastructure calls AddQueryHandlers() which validates dependencies
        builder.Services.AddApplicationServices();

        // Register gRPC authentication interceptors
        builder.Services.AddScoped<JwtAuthenticationInterceptor>();
        builder.Services.AddScoped<CertificateAuthenticationInterceptor>();
        
        // Register gRPC with authentication interceptors
        builder.Services.AddGrpc(options =>
        {
            // Add authentication interceptors globally
            options.Interceptors.Add<JwtAuthenticationInterceptor>();
            options.Interceptors.Add<CertificateAuthenticationInterceptor>();
        });

        // Register SignalR
        builder.Services.AddSignalR();

        return builder
            .AddEmail()  // Register email service (Mamey.Emails) - this is on IMameyBuilder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
            .AddMongoDb()
            .AddPostgresDb()
            .AddHandlersLogging()
            .AddSecurity()  // Register Mamey.Security for encryption/hashing
            .AddMinio()  // Register Mamey.Persistence.Minio (generic services)
            .AddMinIOStorage()  // Register service-specific MinIO storage services
            .AddMicroserviceSharedInfrastructure()  // ← Services must be registered BEFORE this (includes .AddJwt())
            .AddApplication()
            ;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEventMapper, EventMapper>();
        services
            .AddServiceClients();
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
    {
        builder
            .UseSharedInfrastructure()
            .UseNotificationPostgres()
            .UseRabbitMq()
            .AddSubscriptions();
        return builder;
    }
}

