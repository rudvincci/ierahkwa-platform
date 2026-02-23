using Mamey.Government.Shared.Services.DocumentGeneration;
using Mamey.Government.Shared.Services.Mrz;
using Mamey.Government.Shared.Services.Standards;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Shared.Services;

public static class Extensions
{
    public static IServiceCollection AddGovernmentSharedServices(this IServiceCollection services)
    {
        // Register document generation services
        services.AddSingleton<IDocumentNumberGenerator, DocumentNumberGenerator>();
        services.AddSingleton<IStandardsComplianceValidator, StandardsComplianceValidator>();
        services.AddSingleton<IMrzGenerator, MrzGenerator>();
        
        return services;
    }
}
