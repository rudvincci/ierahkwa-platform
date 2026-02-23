using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Infrastructure.MinIO.Services;
using Mamey.FWID.Identities.Infrastructure.Storage;
using Mamey.Microservice.Infrastructure;
using Mamey.Persistence.Minio;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mamey.FWID.Identities.Infrastructure.MinIO;

internal static class Extensions
{
    /// <summary>
    /// Adds MinIO storage services for the Identities service.
    /// </summary>
    /// <param name="builder">The Mamey builder.</param>
    /// <returns>The Mamey builder for chaining.</returns>
    public static IMameyBuilder AddMinIOStorage(this IMameyBuilder builder)
    {
        // MinIO services are already registered by AddMinio() in Infrastructure/Extensions.cs
        // We just need to register our service-specific implementations

        // Register biometric encryption and audit services
        builder.Services.AddScoped<IBiometricEncryptionService, BiometricEncryptionService>();
        builder.Services.AddScoped<IBiometricAuditService, BiometricAuditService>();

        // Register BiometricStorageService (depends on encryption and audit services)
        builder.Services.AddScoped<IBiometricStorageService, BiometricStorageService>();
        builder.Services.AddScoped<Mamey.FWID.Identities.Application.Services.IBiometricEvidenceService, Mamey.FWID.Identities.Application.Services.BiometricEvidenceService>();

        // Register bucket initialization service
        builder.Services.AddHostedService<BucketInitializationService>();

        return builder;
    }
}


