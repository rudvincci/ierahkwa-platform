namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Service for generating QR codes with DID and verification URL.
/// </summary>
internal interface IQrCodeGenerator
{
    /// <summary>
    /// Generates a QR code as a base64-encoded image.
    /// </summary>
    Task<string> GenerateQrCodeAsync(
        string did,
        string verificationUrl,
        CancellationToken cancellationToken = default);
}
