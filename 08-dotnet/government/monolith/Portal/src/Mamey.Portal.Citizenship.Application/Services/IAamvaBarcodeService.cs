using Mamey.AmvvaStandards;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for generating AAMVA-compliant PDF417 barcodes for ID cards
/// </summary>
public interface IAamvaBarcodeService
{
    /// <summary>
    /// Generates a PDF417 barcode image for a driver license card
    /// </summary>
    /// <param name="card">Driver license card (will be validated and sanitized)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF417 barcode image bytes (PNG format), or null if generation fails</returns>
    Task<byte[]?> GenerateDriverLicenseBarcodeAsync(DriverLicenseCard card, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a PDF417 barcode image for an identification card
    /// </summary>
    /// <param name="card">Identification card (will be validated and sanitized)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF417 barcode image bytes (PNG format), or null if generation fails</returns>
    Task<byte[]?> GenerateIdentificationCardBarcodeAsync(IdentificationCard card, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a PDF417 barcode image from AAMVA-encoded data string
    /// </summary>
    /// <param name="aamvaEncodedData">AAMVA-encoded data string (from FieldEncodingHelper)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>PDF417 barcode image bytes (PNG format), or null if generation fails</returns>
    Task<byte[]?> GenerateBarcodeFromEncodedDataAsync(string aamvaEncodedData, CancellationToken cancellationToken = default);
}


