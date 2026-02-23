using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mamey;
using Mamey.Barcode;
using Mamey.Barcode.Http;
using Mamey.Http;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Citizenship.Infrastructure.Persistence.Repositories;
using Mamey.Portal.Citizenship.Infrastructure.Services;
using Mamey.Portal.Citizenship.Infrastructure.Storage;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Citizenship.Domain.Repositories;
using Mamey.Portal.Shared.Storage;
using Mamey.Word;

namespace Mamey.Portal.Citizenship.Infrastructure;

public static class Extensions
{
    public static IMameyBuilder AddCitizenshipInfrastructure(
        this IMameyBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PortalDb")
            ?? "Host=localhost;Database=mamey_portal_dev;Username=postgres;Password=postgres";

        // 1. Ensure logging is registered
        if (!builder.Services.Any(s => s.ServiceType == typeof(ILoggerFactory)))
        {
            builder.Services.AddLogging();
        }

        // 2. Register DbContext
        builder.Services.AddDbContext<CitizenshipDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            var enableSensitiveLogging = builder.Configuration.GetValue<bool>("Logging:EnableSensitiveDataLogging");
            if (enableSensitiveLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        // 3. Register Application Services (interfaces in Application, implementations currently in Infrastructure)
        RegisterApplicationServices(builder.Services);

        // 4. Register Repository Services
        RegisterRepositories(builder.Services);

        // 5. Register Storage Services (MinIO)
        RegisterStorageServices(builder.Services);

        // 6. Register Infrastructure Services
        RegisterInfrastructureServices(builder);

        // 7. Register Background Services
        builder.Services.AddHostedService<ApplicationWorkflowBackgroundService>();

        return builder;
    }

    private static void RegisterApplicationServices(IServiceCollection services)
    {
        // Application services with Infrastructure store implementations.
        services.AddScoped<ICitizenshipApplicationService, CitizenshipApplicationService>();
        services.AddScoped<IApplicationSubmissionStore, ApplicationSubmissionStore>();
        services.AddScoped<ICitizenshipBackofficeService, CitizenshipBackofficeService>();
        services.AddScoped<ICitizenshipBackofficeStore, CitizenshipBackofficeStore>();
        services.AddScoped<IApplicationFormPdfGenerator, ApplicationFormPdfGenerator>();
        services.AddScoped<IApplicationWorkflowService, ApplicationWorkflowService>();
        services.AddScoped<IApplicationWorkflowStore, ApplicationWorkflowStore>();
        services.AddScoped<IApplicationStatusService, ApplicationStatusService>();
        services.AddScoped<IApplicationStatusStore, ApplicationStatusStore>();
        services.AddScoped<IPublicDocumentValidationService, PublicDocumentValidationService>();
        services.AddScoped<IPublicDocumentValidationStore, PublicDocumentValidationStore>();
        services.AddScoped<IPaymentPlanService, PaymentPlanService>();
        services.AddScoped<IPaymentPlanStore, PaymentPlanStore>();
        services.AddScoped<ICitizenshipStatusService, CitizenshipStatusService>();
        services.AddScoped<ICitizenshipStatusStore, CitizenshipStatusStore>();
        services.AddScoped<ICitizenPortalService, CitizenshipCitizenPortalService>();
        services.AddScoped<ICitizenPortalStore, CitizenPortalStore>();
        services.AddScoped<IStandardsComplianceValidator, StandardsComplianceValidator>();
        services.AddScoped<IDocumentNumberGenerator, DocumentNumberGenerator>();
        services.AddScoped<IMrzGenerator, MrzGenerator>();
        services.AddScoped<IDocumentValidityCalculator, DocumentValidityCalculator>();
        services.AddScoped<IStatusProgressionService, StatusProgressionService>();
        services.AddScoped<IStatusProgressionStore, StatusProgressionStore>();

        // NOTE: Realtime services (ICitizenshipRealtimeNotifier, ICitizenshipRealtimeClient)
        // are registered in Web project as they depend on SignalR
    }

    private static void RegisterStorageServices(IServiceCollection services)
    {
        // MinIO storage using Mamey.Persistence.Minio
        // Already configured in Mamey.Portal.Shared
        services.AddScoped<MinioObjectStorage>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IApplicationRepository, PostgresApplicationRepository>();
        services.AddScoped<IIssuedDocumentRepository, PostgresIssuedDocumentRepository>();
        services.AddScoped<IUploadRepository, PostgresUploadRepository>();
        services.AddScoped<IIntakeReviewRepository, PostgresIntakeReviewRepository>();
        services.AddScoped<IPaymentPlanRepository, PostgresPaymentPlanRepository>();
        services.AddScoped<ICitizenshipStatusRepository, PostgresCitizenshipStatusRepository>();
        services.AddScoped<IStatusProgressionRepository, PostgresStatusProgressionRepository>();
    }

    private static void RegisterInfrastructureServices(IMameyBuilder builder)
    {
        var barcodeApiUrl = builder.Configuration["Barcode:ApiUrl"] ?? "http://localhost:18648";
        builder.AddGenericHttpClient<MameyBarcodeApiClient>("MameyBarcodeApiClient", configureClient: config =>
        {
            config.BaseAddress = new Uri(barcodeApiUrl);
        });

        builder.Services.AddSingleton(new BarcodeOptions());
        builder.Services.AddScoped<IMameyBarcodeApiClientResponseHandler, MameyBarcodeApiClientResponseHandler>();
        builder.Services.AddScoped<IMameyBarcodeApiClient>(sp => sp.GetRequiredService<MameyBarcodeApiClient>());
        builder.Services.AddScoped<IBarcodeService, BarcodeService>();
        builder.Services.AddScoped<IAamvaBarcodeService, AamvaBarcodeService>();

        // Barcode services
        builder.Services.AddScoped<IBarcodeScanningService, BarcodeScanningService>();

        builder.Services.AddWord();
    }
}
