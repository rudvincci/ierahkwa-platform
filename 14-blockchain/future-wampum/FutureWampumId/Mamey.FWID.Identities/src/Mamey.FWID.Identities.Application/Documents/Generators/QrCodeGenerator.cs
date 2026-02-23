using Mamey.Barcode;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Service for generating QR codes with DID and verification URL using Mamey.Barcode.
/// </summary>
internal class QrCodeGenerator : IQrCodeGenerator
{
    private readonly IBarcodeService _barcodeService;
    private readonly ILogger<QrCodeGenerator> _logger;

    public QrCodeGenerator(
        IBarcodeService barcodeService,
        ILogger<QrCodeGenerator> logger)
    {
        _barcodeService = barcodeService;
        _logger = logger;
    }

    public async Task<string> GenerateQrCodeAsync(
        string did,
        string verificationUrl,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Combine DID and verification URL in QR code data
            var qrData = $"{did}|{verificationUrl}";

            _logger.LogDebug(
                "Generating QR code using Mamey.Barcode: Data={QrData}",
                qrData);

            // Generate QR code using Mamey.Barcode service
            var qrCodeBytes = await _barcodeService.GenerateQRCodeAsync(
                qrData,
                maxWidth: 500,
                maxHeight: 500);

            if (qrCodeBytes == null || qrCodeBytes.Length == 0)
            {
                _logger.LogWarning(
                    "QR code generation returned empty result for DID: {Did}",
                    did);
                throw new InvalidOperationException("Failed to generate QR code");
            }

            var base64 = Convert.ToBase64String(qrCodeBytes);
            var dataUri = $"data:image/png;base64,{base64}";

            _logger.LogDebug(
                "Successfully generated QR code for DID: {Did}, Size: {Size} bytes",
                did,
                qrCodeBytes.Length);

            return dataUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate QR code for DID: {Did}", did);
            throw;
        }
    }
}
