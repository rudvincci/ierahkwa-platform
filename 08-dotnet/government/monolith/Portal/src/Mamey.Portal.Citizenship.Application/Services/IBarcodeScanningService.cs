namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for scanning and parsing PDF417 barcodes from ID card images
/// </summary>
public interface IBarcodeScanningService
{
    /// <summary>
    /// Scans a PDF417 barcode from an image and extracts the raw barcode data
    /// </summary>
    /// <param name="imageBytes">Image bytes (JPEG, PNG, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw barcode data string, or null if no barcode found</returns>
    Task<string?> ScanBarcodeFromImageAsync(byte[] imageBytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses AAMVA-encoded barcode data to extract the document number
    /// </summary>
    /// <param name="aamvaData">AAMVA-encoded barcode data string</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Document number if found, or null</returns>
    Task<string?> ParseDocumentNumberFromAamvaDataAsync(string aamvaData, CancellationToken cancellationToken = default);
}

