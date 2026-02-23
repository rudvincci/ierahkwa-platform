using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.Common;

namespace Mamey.Portal.Citizenship.Application.Services;

public sealed class BarcodeScanningService : IBarcodeScanningService
{
    private readonly ILogger<BarcodeScanningService> _logger;

    public BarcodeScanningService(ILogger<BarcodeScanningService> logger)
    {
        _logger = logger;
    }
    public async Task<string?> ScanBarcodeFromImageAsync(byte[] imageBytes, CancellationToken cancellationToken = default)
    {
        if (imageBytes == null || imageBytes.Length == 0)
        {
            return null;
        }

        return await Task.Run(() =>
        {
            try
            {
                // Load image using ImageSharp and convert to RGB byte array
                using var image = Image.Load<Rgba32>(imageBytes);
                
                // Convert ImageSharp image to RGB byte array for ZXing
                var width = image.Width;
                var height = image.Height;
                var rgbBytes = new byte[width * height * 3];
                var index = 0;
                
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);
                        for (int x = 0; x < pixelRow.Length; x++)
                        {
                            var pixel = pixelRow[x];
                            rgbBytes[index++] = pixel.R;
                            rgbBytes[index++] = pixel.G;
                            rgbBytes[index++] = pixel.B;
                        }
                    }
                });
                
                // Create luminance source from RGB bytes
                var luminanceSource = new RGBLuminanceSource(rgbBytes, width, height, RGBLuminanceSource.BitmapFormat.RGB24);
                
                // Create barcode reader for PDF417
                var reader = new BarcodeReaderGeneric
                {
                    Options = new DecodingOptions
                    {
                        PossibleFormats = new[] { BarcodeFormat.PDF_417 },
                        TryHarder = true,
                        PureBarcode = false
                    }
                };

                // Try to read the barcode
                var result = reader.Decode(luminanceSource);
                
                return result?.Text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning PDF417 barcode from image");
                return null;
            }
        }, cancellationToken);
    }

    public Task<string?> ParseDocumentNumberFromAamvaDataAsync(string aamvaData, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(aamvaData))
        {
            return Task.FromResult<string?>(null);
        }

        // AAMVA format structure:
        // @\nANSI [version] [issuer ID] [data fields]\n
        // Fields are separated by field separators (typically \n or specific delimiters)
        // Subfields within a field are separated by subfield separators
        // Document number is in field DAQ (Driver License/ID Number), subfield 1
        // Format: DAQ[document number] or DAQ[subfield1][subfield2]...
        
        try
        {
            // Method 1: Look for DAQ field (most common for document number)
            // DAQ can appear as "DAQ" followed by the document number
            // It may also have subfields separated by delimiters
            var daqPatterns = new[]
            {
                @"DAQ([A-Z0-9\-]{6,20})",  // DAQ followed directly by document number
                @"DAQ\s*([A-Z0-9\-]{6,20})",  // DAQ with optional whitespace
                @"DAQ[^\w]*([A-Z0-9\-]{6,20})",  // DAQ with non-word characters
            };

            foreach (var pattern in daqPatterns)
            {
                var match = Regex.Match(aamvaData, pattern, RegexOptions.IgnoreCase);
                if (match.Success && match.Groups.Count > 1)
                {
                    var docNumber = match.Groups[1].Value.Trim();
                    if (IsValidDocumentNumber(docNumber))
                    {
                        return Task.FromResult<string?>(docNumber);
                    }
                }
            }

            // Method 2: Parse AAMVA structure more systematically
            // AAMVA data typically starts with "@\nANSI" or just contains field codes
            // Look for field codes that might contain document numbers
            var fieldCodePattern = @"(DAQ|DAA|DAB|DAC|DAD|DAE|DAF|DAG|DAH|DAI|DAJ|DAK|DAL|DAM|DAN|DAO|DAP|DAR|DAS|DAT|DAU|DAV|DAW|DAX|DAY|DAZ)[^\w]*([A-Z0-9\-]{6,20})";
            var fieldMatches = Regex.Matches(aamvaData, fieldCodePattern, RegexOptions.IgnoreCase);
            
            // Prioritize DAQ (document number), but also check other relevant fields
            var priorityFields = new[] { "DAQ", "DAA", "DAB" }; // DAQ = DLN, DAA/DAB might be alternate IDs
            
            foreach (var priorityField in priorityFields)
            {
                foreach (Match fieldMatch in fieldMatches)
                {
                    if (fieldMatch.Groups.Count >= 3 && 
                        fieldMatch.Groups[1].Value.Equals(priorityField, StringComparison.OrdinalIgnoreCase))
                    {
                        var docNumber = fieldMatch.Groups[2].Value.Trim();
                        if (IsValidDocumentNumber(docNumber))
                        {
                            return Task.FromResult<string?>(docNumber);
                        }
                    }
                }
            }

            // Method 3: Fallback - look for patterns that look like document numbers
            // This is less reliable but may catch cases where field codes aren't present
            var fallbackPattern = @"\b([A-Z]{1,3}[-]?[A-Z0-9]{5,17})\b";
            var fallbackMatches = Regex.Matches(aamvaData, fallbackPattern);
            
            foreach (Match match in fallbackMatches)
            {
                var candidate = match.Groups[1].Value;
                if (IsValidDocumentNumber(candidate))
                {
                    return Task.FromResult<string?>(candidate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing AAMVA data to extract document number");
        }

        return Task.FromResult<string?>(null);
    }

    private static bool IsValidDocumentNumber(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        candidate = candidate.Trim();

        // Must be at least 6 characters (typical minimum for document numbers)
        if (candidate.Length < 6 || candidate.Length > 20)
        {
            return false;
        }

        // Skip if it looks like a date (YYYYMMDD format)
        if (Regex.IsMatch(candidate, @"^\d{8}$"))
        {
            if (int.TryParse(candidate.Substring(0, 4), out var year) && year >= 1900 && year <= 2100)
            {
                return false;
            }
        }

        // Skip if it's just a state code (2 letters)
        if (candidate.Length == 2 && Regex.IsMatch(candidate, @"^[A-Z]{2}$", RegexOptions.IgnoreCase))
        {
            return false;
        }

        // Skip if it's all digits and looks like a date or SSN
        if (Regex.IsMatch(candidate, @"^\d{9}$") || Regex.IsMatch(candidate, @"^\d{11}$"))
        {
            return false;
        }

        // Should contain alphanumeric characters (may include hyphens)
        if (!Regex.IsMatch(candidate, @"^[A-Z0-9\-]+$", RegexOptions.IgnoreCase))
        {
            return false;
        }

        return true;
    }
}

