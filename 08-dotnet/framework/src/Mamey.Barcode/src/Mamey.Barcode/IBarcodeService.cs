using Mamey.Barcode.Requests;

namespace Mamey.Barcode;

public interface IBarcodeService
{
    // void GenerateQRCode(string data);
    Task<byte[]?> GenerateQRCodeAsync(string data, int maxWidth = 500, int maxHeight = 500);
    
    /// <summary>
    /// Generates a PDF417 barcode image from the provided data.
    /// </summary>
    /// <param name="data">The data to encode in the PDF417 barcode</param>
    /// <param name="columns">Number of columns in the PDF417 barcode (default: 20)</param>
    /// <param name="securityLevel">Error correction level 0-8 (default: 5 for AAMVA compliance)</param>
    /// <param name="scale">Scale factor for barcode elements (default: 2)</param>
    /// <param name="ratio">Height-to-width ratio (default: 3)</param>
    /// <param name="padding">Padding around the barcode (default: 5)</param>
    /// <returns>Byte array containing the PDF417 barcode image (PNG format), or null if generation fails</returns>
    Task<byte[]?> GeneratePDF417Async(
        string data,
        int columns = 20,
        int securityLevel = 5,
        int scale = 2,
        int ratio = 3,
        int padding = 5);
}
