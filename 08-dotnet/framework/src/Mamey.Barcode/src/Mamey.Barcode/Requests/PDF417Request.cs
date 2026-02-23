using System.Text.Json.Serialization;

namespace Mamey.Barcode.Requests;

public class PDF417Request : BarcodeRequest
{
    public PDF417Request(string data, int columns = 20,
        int securityLevel = 4, int scale = 2, int ratio =3, int padding = 5,
        string format="png", string foregroundColor = "#000000", string backGroundColor = "#FFFFFF")
        : base(data, BarCodeType.PDF417.GetEnumShortNameDisplayAttribute())
    {
        Columns = columns;
        SecurityLevel = securityLevel;
        Scale = scale;
        Ratio = ratio;
        Padding = padding;
        Format = format;
        ForegroundColor = foregroundColor;
        BackGroundColor = backGroundColor;
    }
    /// <summary>
    /// Specifies the number of columns in the PDF417 barcode.
    /// This parameter directly influences the barcode's width.
    /// A higher number of columns will result in a wider barcode.
    /// </summary>
    public int Columns { get; set; }
    /// <summary>
    /// Determines the error correction capacity of the barcode.
    /// The PDF417 barcode standard supports error correction
    /// levels ranging from 0 to 8, referred to here as the
    /// security level.
    /// </summary>
    [JsonPropertyName("security_level")]
    public int SecurityLevel { get; set; }
    /// <summary>
    /// The scale factor controls the size of individual barcode
    /// elements (modules). Essentially, it magnifies or reduces
    /// the barcode without altering its proportions or encoded data.
    /// </summary>
    public int Scale { get; set; }
    /// <summary>
    /// Dictates the height-to-width ratio of the individual barcode
    /// elements. This can adjust the barcode's aspect ratio without
    /// changing the encoded data.
    /// </summary>
    public int Ratio { get; set; }
    /// <summary>
    /// Sets the padding of the image area
    /// </summary>
    public int Padding { get; set; }
    /// <summary>
    /// Specifies the output format of the generated barcode image. Available options are PNG and SVG.
    /// </summary>
    public string Format { get; set; }
    // Foreground color of the barcode. Default is black.
    [JsonPropertyName("fg_color")]
    public string ForegroundColor { get; set; }
    /// <summary>
    /// Background color of the barcode. Default is white.
    /// </summary>
    [JsonPropertyName("bg_color")]
    public string BackGroundColor { get; set; }
}

