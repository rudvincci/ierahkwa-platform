using System.Text.RegularExpressions;

namespace Mamey.ISO.SWIFT;

/// <summary>
/// Block 1: Basic Header Block
/// Format: {1:APPLICATION_IDORIGINATOR_ADDRESS_SESSION_NUMBER_SEQUENCE_NUMBER}
/// </summary>
public class SwiftBlock1
{
    /// <summary>
    /// Application ID (F = FIN, A = GPA, L = GPA)
    /// </summary>
    public string ApplicationId { get; set; } = string.Empty;

    /// <summary>
    /// Service ID (01 = FIN, 21 = GPA)
    /// </summary>
    public string ServiceId { get; set; } = string.Empty;

    /// <summary>
    /// Logical Terminal Address (12 characters: 4 char bank code + 1 char country + 1 char location + 3 char branch + 3 char terminal)
    /// </summary>
    public string LogicalTerminalAddress { get; set; } = string.Empty;

    /// <summary>
    /// Session Number (4 digits)
    /// </summary>
    public string SessionNumber { get; set; } = string.Empty;

    /// <summary>
    /// Sequence Number (6 digits)
    /// </summary>
    public string SequenceNumber { get; set; } = string.Empty;

    public static SwiftBlock1 Parse(string block1Content)
    {
        if (string.IsNullOrWhiteSpace(block1Content) || block1Content.Length < 25)
            throw new ArgumentException("Invalid Block 1 format", nameof(block1Content));

        return new SwiftBlock1
        {
            ApplicationId = block1Content.Substring(0, 1),
            ServiceId = block1Content.Substring(1, 2),
            LogicalTerminalAddress = block1Content.Substring(3, 12),
            SessionNumber = block1Content.Substring(15, 4),
            SequenceNumber = block1Content.Substring(19, 6)
        };
    }
}

/// <summary>
/// Block 2: Application Header Block
/// Format: {2:INPUT_OUTPUT_IDMESSAGE_TYPE_RECEIVER_ADDRESS_MESSAGE_PRIORITY_DELIVERY_MONITORING_POSSIBLE_DUPLICATE_EMISSION}
/// </summary>
public class SwiftBlock2
{
    /// <summary>
    /// Input/Output ID (I = Input, O = Output)
    /// </summary>
    public string InputOutputId { get; set; } = string.Empty;

    /// <summary>
    /// Message Type (e.g., 103, 202, 940)
    /// </summary>
    public string MessageType { get; set; } = string.Empty;

    /// <summary>
    /// Receiver Address (12 characters BIC)
    /// </summary>
    public string ReceiverAddress { get; set; } = string.Empty;

    /// <summary>
    /// Message Priority (S = System, U = Urgent, N = Normal)
    /// </summary>
    public string MessagePriority { get; set; } = string.Empty;

    /// <summary>
    /// Delivery Monitoring (1, 2, 3, or blank)
    /// </summary>
    public string DeliveryMonitoring { get; set; } = string.Empty;

    /// <summary>
    /// Obsolescence Period (HHMM format or blank)
    /// </summary>
    public string ObsolescencePeriod { get; set; } = string.Empty;

    public static SwiftBlock2 Parse(string block2Content)
    {
        if (string.IsNullOrWhiteSpace(block2Content))
            throw new ArgumentException("Invalid Block 2 format", nameof(block2Content));

        var block = new SwiftBlock2
        {
            InputOutputId = block2Content.Substring(0, 1)
        };

        // Input message format
        if (block.InputOutputId == "I")
        {
            if (block2Content.Length >= 28)
            {
                block.MessageType = block2Content.Substring(1, 3);
                block.ReceiverAddress = block2Content.Substring(4, 12);
                block.MessagePriority = block2Content.Length > 16 ? block2Content.Substring(16, 1) : string.Empty;
                block.DeliveryMonitoring = block2Content.Length > 17 ? block2Content.Substring(17, 1) : string.Empty;
                block.ObsolescencePeriod = block2Content.Length > 18 ? block2Content.Substring(18, 4) : string.Empty;
            }
        }
        // Output message format
        else if (block.InputOutputId == "O")
        {
            if (block2Content.Length >= 20)
            {
                block.MessageType = block2Content.Substring(1, 3);
                block.MessagePriority = block2Content.Length > 4 ? block2Content.Substring(4, 1) : string.Empty;
                block.DeliveryMonitoring = block2Content.Length > 5 ? block2Content.Substring(5, 1) : string.Empty;
            }
        }

        return block;
    }
}

/// <summary>
/// Block 3: User Header Block (Optional)
/// Format: {3:{TAG:VALUE}{TAG:VALUE}...}
/// </summary>
public class SwiftBlock3
{
    public Dictionary<string, string> Tags { get; set; } = new();

    public static SwiftBlock3 Parse(string block3Content)
    {
        var block3 = new SwiftBlock3();

        if (string.IsNullOrWhiteSpace(block3Content))
            return block3;

        // Parse tags in format {TAG:VALUE}
        var tagPattern = @"\{(\d{3}[A-Z]?):([^}]+)\}";
        var matches = Regex.Matches(block3Content, tagPattern);

        foreach (Match match in matches)
        {
            var tag = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            block3.Tags[tag] = value;
        }

        return block3;
    }
}

/// <summary>
/// Block 4: Text Block (Message Body)
/// Format: {4: :TAG:VALUE :TAG:VALUE ... -}
/// </summary>
public class SwiftBlock4
{
    public List<SwiftField> Fields { get; set; } = new();

    public static SwiftBlock4 Parse(string block4Content)
    {
        var block4 = new SwiftBlock4();

        if (string.IsNullOrWhiteSpace(block4Content))
            return block4;

        // Remove the leading space and trailing dash
        var content = block4Content.TrimStart(' ').TrimEnd('-', ' ');

        // Parse fields in format :TAG:VALUE
        // Handle multi-line fields and continuation
        var fieldPattern = @":(\d{2}[A-Z]?):([^:]+?)(?=:\d{2}[A-Z]?:|$)";
        var matches = Regex.Matches(content, fieldPattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            var tag = match.Groups[1].Value;
            var value = match.Groups[2].Value.Trim();

            block4.Fields.Add(new SwiftField
            {
                Tag = tag,
                Value = value
            });
        }

        return block4;
    }
}

/// <summary>
/// Block 5: Trailer Block (Optional)
/// Format: {5:{TAG:VALUE}{TAG:VALUE}...}
/// </summary>
public class SwiftBlock5
{
    public Dictionary<string, string> Tags { get; set; } = new();

    public static SwiftBlock5 Parse(string block5Content)
    {
        var block5 = new SwiftBlock5();

        if (string.IsNullOrWhiteSpace(block5Content))
            return block5;

        // Parse tags in format {TAG:VALUE}
        var tagPattern = @"\{([A-Z]{3}):([^}]+)\}";
        var matches = Regex.Matches(block5Content, tagPattern);

        foreach (Match match in matches)
        {
            var tag = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            block5.Tags[tag] = value;
        }

        return block5;
    }
}

/// <summary>
/// Represents a field in Block 4
/// </summary>
public class SwiftField
{
    public string Tag { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
