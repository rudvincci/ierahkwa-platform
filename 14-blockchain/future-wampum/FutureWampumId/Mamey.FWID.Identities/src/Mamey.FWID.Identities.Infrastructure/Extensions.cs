using System.Runtime.CompilerServices;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.CQRS.Events;
using Mamey.Microservice.Infrastructure;
using Mamey.Time;
using Mamey.Microservice.Infrastructure.Time;
using Mamey.FWID.Identities.Application;
using Mamey.FWID.Identities.Infrastructure.Clients;
using Mamey.FWID.Identities.Infrastructure.EF;
using Mamey.FWID.Identities.Infrastructure.Exceptions;
using Mamey.FWID.Identities.Infrastructure.Metrics;
using Mamey.FWID.Identities.Infrastructure.Mongo;
using Mamey.FWID.Identities.Infrastructure.Redis;
using Mamey.FWID.Identities.Infrastructure.Composite;
using Mamey.FWID.Identities.Infrastructure.Services;
using Mamey.FWID.Identities.Infrastructure.Logging;
using Mamey.FWID.Identities.Infrastructure.Blockchain;
using Mamey.FWID.Identities.Infrastructure.Compliance;
using Mamey.FWID.Identities.Application.Services;
using Mamey.Auth.DecentralizedIdentifiers;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Mamey.Auth.DecentralizedIdentifiers.Caching;
using Mamey.Security;
using Mamey.WebApi;
using Mamey.WebApi.Security;
using Mamey.FWID.Identities.Infrastructure.Grpc.Interceptors;
using Mamey.FWID.Identities.Infrastructure.MinIO;
using Mamey.FWID.Identities.Infrastructure.Security;
using Mamey.FWID.Identities.Infrastructure.Ledger;
using Mamey.FWID.Identities.Infrastructure.Sync;
using Mamey.Persistence.Minio;
using Mamey.Emails;
using Mamey.Barcode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Api")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.EndToEnd")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Integration")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit")]
[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Shared")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]
namespace Mamey.FWID.Identities.Infrastructure;

internal static class Extensions
{
    public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder, IConfiguration configuration)
    {
        // Ensure logging is registered before handlers are scanned
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }
        
        builder.Services.AddInfrastructure(configuration);
        
        // Register application services BEFORE AddMicroserviceSharedInfrastructure
        // CRITICAL: AddMicroserviceSharedInfrastructure calls AddQueryHandlers() which validates dependencies
        builder.Services.AddApplicationServices();
        
        // Register gRPC authentication interceptors
        builder.Services.AddScoped<JwtAuthenticationInterceptor>();
        builder.Services.AddScoped<CertificateAuthenticationInterceptor>();
        
        // Register gRPC with authentication interceptors
        // Note: gRPC services (BiometricGrpcService, PermissionSyncGrpcService) are registered in Program.cs
        // because they are in the Api project and Infrastructure cannot reference Api
        builder.Services.AddGrpc(options =>
        {
            // Add authentication interceptors globally
            options.Interceptors.Add<JwtAuthenticationInterceptor>();
            options.Interceptors.Add<CertificateAuthenticationInterceptor>();
        });
        
        builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddExceptionToMessageMapper<ExceptionToMessageMapper>()
            .AddMongoDb()
            .AddPostgresDb()
            .AddRedisRepositories()
            .AddCompositeRepositories()
            .AddHandlersLogging()
            .AddMinio()  // Register Mamey.Persistence.Minio (generic services)
            .AddMinIOStorage()  // Register service-specific MinIO storage services
            .AddEmail()  // Register email service (Mamey.Emails) - registers IFluentEmail and EmailOptions
            .AddBarcode()  // Register Mamey.Barcode for QR code generation
            
            .AddDecentralizedIdentifiers()  // Register DID authentication and all dependencies (JWT is already registered via AddMultiAuth)
            .AddMicroserviceSharedInfrastructure()  // ← Services must be registered BEFORE this (includes .AddJwt() and .AddCertificateAuthentication())
            .AddApplication();
        
        // Register custom permission validator AFTER AddMicroserviceSharedInfrastructure
        // This replaces the default validator registered by AddCertificateAuthentication()
        builder.Services.AddSingleton<ICertificatePermissionValidator, IdentityPermissionValidator>();
        
        return builder;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IClock, UtcClock>();
        services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHostedService<MetricsJob>();
        services.AddSingleton<CustomMetricsMiddleware>();
        services
            .AddServiceClients()
            .AddIdentityServices()
            .AddResilientBlockchainServices(configuration)  // Add resilient blockchain with retry/circuit breaker
            .AddComplianceServices(configuration)  // Add AML/KYC compliance audit trail
            .AddLedgerAuditServices()  // Add FutureWampumLedger integration for immutable audit trail
            .AddReadModelSyncServices();  // Add event-driven read model sync (PostgreSQL -> MongoDB)
        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder)
    {
        builder
            .UseSharedInfrastructure()
            .UseAuthentication()  // Add authentication middleware (required for authorization)
            .UseAuthorization()   // Add authorization middleware (required for endpoints with auth metadata)
            .UseIdentityPostgres()
            .UseRabbitMq()
            .AddSubscriptions();
        return builder;
    }
}
