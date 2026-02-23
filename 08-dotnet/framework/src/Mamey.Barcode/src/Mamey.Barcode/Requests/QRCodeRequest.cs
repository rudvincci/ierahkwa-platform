using System.Text.Json.Serialization;

namespace Mamey.Barcode.Requests;

public class QRCodeRequest : BarcodeRequest
{
    public QRCodeRequest(string data, int version = 1,
        string errorCorrection = "L", int boxSize = 10, int border = 4,
        string foregroundColor = "#000000", string backgroundColor = "#FFFFFF",
        string? imageUrl = null)
        : base(data, BarCodeType.QRCode.GetEnumShortNameDisplayAttribute())
    {
        Version = version;
        ErrorCorrection = errorCorrection;
        BoxSize = boxSize;
        Border = border;
        ForegroundColor = foregroundColor;
        BackgroundColor = backgroundColor;
        ImageUrl = imageUrl;
    }
    /// <summary>
    /// QR codes are standardized into versions, indicating the
    /// overall dimensions and capacity of the QR code. Versions
    /// range from 1 to 40, with 1 being the smallest and 40 the
    /// largest.
    /// </summary>
    public int Version { get; set; }
    /// <summary>
    ///  Default Value: Low
    ///  Error correction levels in QR codes help recover data even if the code
    ///  is dirty, damaged, or partially obscured.
    ///  Four levels are available:
    ///  Low (L): Recovers 7% of data.
    ///  Medium(M): Recovers 15% of data.
    ///  Quartile (Q): Recovers 25% of data.
    ///  High (H): Recovers 30% of data.
    ///  Higher error correction levels increase the QR code's resilience at
    ///  the cost of increasing its size. This feature is crucial for ensuring
    ///  the QR code remains functional in challenging environments.
    /// </summary>
    [JsonPropertyName("error_correction")]
    public string ErrorCorrection { get; set; }
    /// <summary>
    /// Specifies the size of each square in the QR code grid, measured in pixels.
    /// </summary>
    [JsonPropertyName("box_size")]
    public int BoxSize { get; set; }
    public int Border { get; set; }
    [JsonPropertyName("fg_color")]
    public string ForegroundColor { get; set; }
    [JsonPropertyName("bg_color")]
    public string BackgroundColor { get; set; }
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

