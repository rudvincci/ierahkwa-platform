using Mamey.FWID.Identities.Application.Documents.Generators;
using Mamey.FWID.Identities.Application.Documents.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.Application.Documents;

/// <summary>
/// Extension methods for registering document generation services.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Adds document generation services for ID cards and passports.
    /// </summary>
    public static IServiceCollection AddDocumentGenerationServices(this IServiceCollection services)
    {
        // Register generators
        services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();
        services.AddScoped<IPdfGenerator, PdfGenerator>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IIdCardGenerator, IdCardGenerator>();
        services.AddScoped<IPassportGenerator, PassportGenerator>();

        // Register storage service
        services.AddScoped<IDocumentStorageService, DocumentStorageService>();

        // Register main document generation service
        services.AddScoped<IDocumentGenerationService, DocumentGenerationService>();

        return services;
    }
}
