using System.Text;

namespace Mamey.ISO.SWIFT;

/// <summary>
/// Main parser for SWIFT FIN messages
/// </summary>
public static class SwiftParser
{
    /// <summary>
    /// Parse a SWIFT FIN message from text
    /// </summary>
    public static SwiftFinMessage Parse(string message)
    {
        return SwiftFinMessage.Parse(message);
    }

    /// <summary>
    /// Parse multiple SWIFT FIN messages from text (separated by newlines or specific delimiters)
    /// </summary>
    public static IEnumerable<SwiftFinMessage> ParseMultiple(string messages)
    {
        var results = new List<SwiftFinMessage>();

        // Try to split by common delimiters
        var messageTexts = messages.Split(new[] { "\r\n\r\n", "\n\n", "{1:" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var messageText in messageTexts)
        {
            var trimmed = messageText.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            // If the message doesn't start with {1:, add it back
            if (!trimmed.StartsWith("{1:"))
                trimmed = "{1:" + trimmed;

            try
            {
                var message = Parse(trimmed);
                results.Add(message);
            }
            catch (Exception ex)
            {
                // Log error but continue parsing other messages
                System.Diagnostics.Debug.WriteLine($"Failed to parse message: {ex.Message}");
            }
        }

        return results;
    }

    /// <summary>
    /// Convert SWIFT FIN message to JSON
    /// </summary>
    public static string ToJson(string swiftMessage, bool prettyPrint = true)
    {
        var message = Parse(swiftMessage);
        return message.ToJson(prettyPrint);
    }

    /// <summary>
    /// Convert SWIFT FIN message to XML
    /// </summary>
    public static string ToXml(string swiftMessage)
    {
        var message = Parse(swiftMessage);
        return message.ToXml();
    }

    /// <summary>
    /// Validate SWIFT FIN message structure
    /// </summary>
    public static SwiftValidationResult Validate(string message)
    {
        var result = new SwiftValidationResult();

        if (string.IsNullOrWhiteSpace(message))
        {
            result.IsValid = false;
            result.Errors.Add("Message is null or empty");
            return result;
        }

        // Check for required blocks
        if (!message.Contains("{1:"))
        {
            result.IsValid = false;
            result.Errors.Add("Missing Block 1 (Basic Header)");
        }

        if (!message.Contains("{2:"))
        {
            result.IsValid = false;
            result.Errors.Add("Missing Block 2 (Application Header)");
        }

        if (!message.Contains("{4:"))
        {
            result.IsValid = false;
            result.Errors.Add("Missing Block 4 (Text Block)");
        }

        // Try to parse and catch any parsing errors
        try
        {
            var parsed = Parse(message);
            
            // Validate Block 1
            if (parsed.Block1 != null)
            {
                if (parsed.Block1.ApplicationId != "F" && parsed.Block1.ApplicationId != "A" && parsed.Block1.ApplicationId != "L")
                {
                    result.Warnings.Add("Block 1: Invalid Application ID (should be F, A, or L)");
                }
            }

            // Validate Block 2
            if (parsed.Block2 != null)
            {
                if (string.IsNullOrEmpty(parsed.Block2.MessageType))
                {
                    result.Errors.Add("Block 2: Message Type is missing");
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(parsed.Block2.MessageType, @"^\d{3}$"))
                {
                    result.Warnings.Add($"Block 2: Message Type '{parsed.Block2.MessageType}' format may be invalid");
                }
            }

            // Validate Block 4 has fields
            if (parsed.Block4 != null && !parsed.Block4.Fields.Any())
            {
                result.Warnings.Add("Block 4: No fields found in message body");
            }
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Parsing error: {ex.Message}");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}

/// <summary>
/// Validation result for SWIFT message
/// </summary>
public class SwiftValidationResult
{
    public bool IsValid { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
