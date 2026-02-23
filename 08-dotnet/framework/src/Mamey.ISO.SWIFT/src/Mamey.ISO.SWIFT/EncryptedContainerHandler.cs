using System.Text;

namespace Mamey.ISO.SWIFT;

/// <summary>
/// Handler for encrypted/compressed SWIFT container files (.fin, .cef, etc.)
/// </summary>
public static class EncryptedContainerHandler
{
    /// <summary>
    /// Attempts to detect if a file is an encrypted SWIFT container
    /// </summary>
    public static bool IsEncryptedContainer(byte[] fileData)
    {
        if (fileData == null || fileData.Length < 16)
            return false;

        // Check for common encryption/compression signatures
        // PlayStation PKG files start with 7f 43 4e 54 (CNT)
        if (fileData[0] == 0x7f && fileData[1] == 0x43 && fileData[2] == 0x4e && fileData[3] == 0x54)
        {
            return false; // This is a PlayStation PKG, not a SWIFT file
        }

        // Check for encrypted binary data (high entropy)
        var entropy = CalculateEntropy(fileData, 0, Math.Min(1024, fileData.Length));
        
        // Encrypted data typically has high entropy (> 7.5)
        // Plain text SWIFT messages have lower entropy
        if (entropy > 7.5)
        {
            return true;
        }

        // Check if it looks like plain text SWIFT
        var text = Encoding.UTF8.GetString(fileData, 0, Math.Min(256, fileData.Length));
        if (text.Contains("{1:") || text.Contains("{2:") || text.Contains("{4:"))
        {
            return false; // This is plain text SWIFT
        }

        return true; // Assume encrypted if we can't determine
    }

    /// <summary>
    /// Attempts to extract SWIFT messages from an encrypted container
    /// Note: This is a placeholder - actual decryption requires keys/protocols
    /// </summary>
    public static async Task<ExtractionResult> ExtractSwiftMessagesAsync(
        byte[] containerData,
        string? decryptionKey = null,
        string? containerFormat = null)
    {
        var result = new ExtractionResult
        {
            IsEncrypted = IsEncryptedContainer(containerData)
        };

        if (!result.IsEncrypted)
        {
            // Try to parse as plain text
            try
            {
                var text = Encoding.UTF8.GetString(containerData);
                var messages = SwiftParser.ParseMultiple(text);
                result.Messages = messages.ToList();
                result.Success = true;
                result.Message = "Container appears to be plain text SWIFT";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Failed to parse as plain text: {ex.Message}";
            }
        }
        else
        {
            // Encrypted container - would need actual decryption logic
            result.Success = false;
            result.Message = "Container appears to be encrypted. Decryption not implemented. " +
                           "You may need to decrypt the file using the appropriate tool/key first.";
            
            // Check for known container formats
            result.DetectedFormat = DetectContainerFormat(containerData);
        }

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Detects the container format based on file signature
    /// </summary>
    private static string? DetectContainerFormat(byte[] data)
    {
        if (data.Length < 16)
            return null;

        // Check for PlayStation PKG
        if (data[0] == 0x7f && data[1] == 0x43 && data[2] == 0x4e && data[3] == 0x54)
        {
            return "PlayStation PKG (CNT)";
        }

        // Check for ZIP/compressed
        if (data[0] == 0x50 && data[1] == 0x4B && (data[2] == 0x03 || data[2] == 0x05 || data[2] == 0x07))
        {
            return "ZIP Archive";
        }

        // Check for GZIP
        if (data[0] == 0x1F && data[1] == 0x8B)
        {
            return "GZIP Compressed";
        }

        // Check for encrypted binary (high entropy)
        var entropy = CalculateEntropy(data, 0, Math.Min(1024, data.Length));
        if (entropy > 7.5)
        {
            return "Encrypted Binary (High Entropy)";
        }

        return "Unknown Binary Format";
    }

    /// <summary>
    /// Calculates Shannon entropy of data (used to detect encryption)
    /// </summary>
    private static double CalculateEntropy(byte[] data, int offset, int length)
    {
        if (data == null || length == 0)
            return 0;

        var frequency = new int[256];
        var end = Math.Min(offset + length, data.Length);

        for (int i = offset; i < end; i++)
        {
            frequency[data[i]]++;
        }

        double entropy = 0;
        double log2 = Math.Log(2);

        for (int i = 0; i < 256; i++)
        {
            if (frequency[i] > 0)
            {
                double probability = (double)frequency[i] / length;
                entropy -= probability * (Math.Log(probability) / log2);
            }
        }

        return entropy;
    }

    /// <summary>
    /// Reads a file and attempts to extract SWIFT messages
    /// </summary>
    public static async Task<ExtractionResult> ExtractFromFileAsync(
        string filePath,
        string? decryptionKey = null)
    {
        if (!File.Exists(filePath))
        {
            return new ExtractionResult
            {
                Success = false,
                Message = $"File not found: {filePath}"
            };
        }

        try
        {
            var fileData = await File.ReadAllBytesAsync(filePath);
            return await ExtractSwiftMessagesAsync(fileData, decryptionKey);
        }
        catch (Exception ex)
        {
            return new ExtractionResult
            {
                Success = false,
                Message = $"Error reading file: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Result of extraction attempt
/// </summary>
public class ExtractionResult
{
    public bool Success { get; set; }
    public bool IsEncrypted { get; set; }
    public string? Message { get; set; }
    public string? DetectedFormat { get; set; }
    public List<SwiftFinMessage> Messages { get; set; } = new();
}
