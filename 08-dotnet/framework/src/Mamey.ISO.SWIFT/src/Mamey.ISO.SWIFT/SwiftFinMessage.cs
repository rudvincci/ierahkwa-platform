using System.Text;
using System.Text.RegularExpressions;

namespace Mamey.ISO.SWIFT;

/// <summary>
/// Represents a complete SWIFT FIN message with all blocks (1-5)
/// </summary>
public class SwiftFinMessage
{
    /// <summary>
    /// Block 1: Basic Header Block
    /// Format: {1:APPLICATION_IDORIGINATOR_ADDRESS_SESSION_NUMBER_SEQUENCE_NUMBER}
    /// </summary>
    public SwiftBlock1? Block1 { get; set; }

    /// <summary>
    /// Block 2: Application Header Block
    /// Format: {2:INPUT_OUTPUT_IDMESSAGE_TYPE_RECEIVER_ADDRESS_MESSAGE_PRIORITY_DELIVERY_MONITORING_POSSIBLE_DUPLICATE_EMISSION}
    /// </summary>
    public SwiftBlock2? Block2 { get; set; }

    /// <summary>
    /// Block 3: User Header Block (Optional)
    /// Format: {3:{TAG:VALUE}{TAG:VALUE}...}
    /// </summary>
    public SwiftBlock3? Block3 { get; set; }

    /// <summary>
    /// Block 4: Text Block (Message Body)
    /// Format: {4: :TAG:VALUE :TAG:VALUE ... -}
    /// </summary>
    public SwiftBlock4? Block4 { get; set; }

    /// <summary>
    /// Block 5: Trailer Block (Optional)
    /// Format: {5:{TAG:VALUE}{TAG:VALUE}...}
    /// </summary>
    public SwiftBlock5? Block5 { get; set; }

    /// <summary>
    /// Raw message text
    /// </summary>
    public string RawMessage { get; set; } = string.Empty;

    /// <summary>
    /// Message Type (e.g., MT103, MT202, MT940)
    /// </summary>
    public string? MessageType { get; set; }

    /// <summary>
    /// Parse a SWIFT FIN message from text
    /// </summary>
    public static SwiftFinMessage Parse(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        var swiftMessage = new SwiftFinMessage
        {
            RawMessage = message
        };

        // Parse Block 1: Basic Header
        var block1Match = Regex.Match(message, @"\{1:([^}]+)\}");
        if (block1Match.Success)
        {
            swiftMessage.Block1 = SwiftBlock1.Parse(block1Match.Groups[1].Value);
        }

        // Parse Block 2: Application Header
        var block2Match = Regex.Match(message, @"\{2:([^}]+)\}");
        if (block2Match.Success)
        {
            swiftMessage.Block2 = SwiftBlock2.Parse(block2Match.Groups[1].Value);
            swiftMessage.MessageType = swiftMessage.Block2.MessageType;
        }

        // Parse Block 3: User Header (Optional)
        var block3Match = Regex.Match(message, @"\{3:([^}]+)\}");
        if (block3Match.Success)
        {
            swiftMessage.Block3 = SwiftBlock3.Parse(block3Match.Groups[1].Value);
        }

        // Parse Block 4: Text Block
        var block4Match = Regex.Match(message, @"\{4:([^}]+)\}");
        if (block4Match.Success)
        {
            swiftMessage.Block4 = SwiftBlock4.Parse(block4Match.Groups[1].Value);
        }

        // Parse Block 5: Trailer (Optional)
        var block5Match = Regex.Match(message, @"\{5:([^}]+)\}");
        if (block5Match.Success)
        {
            swiftMessage.Block5 = SwiftBlock5.Parse(block5Match.Groups[1].Value);
        }

        return swiftMessage;
    }

    /// <summary>
    /// Convert to JSON format
    /// </summary>
    public string ToJson(bool prettyPrint = true)
    {
        var options = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = prettyPrint,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };

        return System.Text.Json.JsonSerializer.Serialize(this, options);
    }

    /// <summary>
    /// Convert to XML format
    /// </summary>
    public string ToXml()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<SwiftFinMessage>");

        if (Block1 != null)
        {
            sb.AppendLine("  <Block1>");
            sb.AppendLine($"    <ApplicationId>{Block1.ApplicationId}</ApplicationId>");
            sb.AppendLine($"    <ServiceId>{Block1.ServiceId}</ServiceId>");
            sb.AppendLine($"    <LogicalTerminalAddress>{Block1.LogicalTerminalAddress}</LogicalTerminalAddress>");
            sb.AppendLine($"    <SessionNumber>{Block1.SessionNumber}</SessionNumber>");
            sb.AppendLine($"    <SequenceNumber>{Block1.SequenceNumber}</SequenceNumber>");
            sb.AppendLine("  </Block1>");
        }

        if (Block2 != null)
        {
            sb.AppendLine("  <Block2>");
            sb.AppendLine($"    <InputOutputId>{Block2.InputOutputId}</InputOutputId>");
            sb.AppendLine($"    <MessageType>{Block2.MessageType}</MessageType>");
            sb.AppendLine($"    <ReceiverAddress>{Block2.ReceiverAddress}</ReceiverAddress>");
            sb.AppendLine($"    <MessagePriority>{Block2.MessagePriority}</MessagePriority>");
            sb.AppendLine($"    <DeliveryMonitoring>{Block2.DeliveryMonitoring}</DeliveryMonitoring>");
            sb.AppendLine($"    <ObsolescencePeriod>{Block2.ObsolescencePeriod}</ObsolescencePeriod>");
            sb.AppendLine("  </Block2>");
        }

        if (Block3 != null && Block3.Tags.Any())
        {
            sb.AppendLine("  <Block3>");
            foreach (var tag in Block3.Tags)
            {
                sb.AppendLine($"    <Tag id=\"{tag.Key}\">{tag.Value}</Tag>");
            }
            sb.AppendLine("  </Block3>");
        }

        if (Block4 != null && Block4.Fields.Any())
        {
            sb.AppendLine("  <Block4>");
            foreach (var field in Block4.Fields)
            {
                sb.AppendLine($"    <Field tag=\"{field.Tag}\">{System.Security.SecurityElement.Escape(field.Value)}</Field>");
            }
            sb.AppendLine("  </Block4>");
        }

        if (Block5 != null && Block5.Tags.Any())
        {
            sb.AppendLine("  <Block5>");
            foreach (var tag in Block5.Tags)
            {
                sb.AppendLine($"    <Tag id=\"{tag.Key}\">{tag.Value}</Tag>");
            }
            sb.AppendLine("  </Block5>");
        }

        sb.AppendLine("</SwiftFinMessage>");
        return sb.ToString();
    }

    /// <summary>
    /// Get field value by tag from Block 4
    /// </summary>
    public string? GetFieldValue(string tag)
    {
        return Block4?.Fields.FirstOrDefault(f => f.Tag == tag)?.Value;
    }

    /// <summary>
    /// Get all fields with a specific tag (for repetitive fields)
    /// </summary>
    public IEnumerable<SwiftField> GetFields(string tag)
    {
        return Block4?.Fields.Where(f => f.Tag == tag) ?? Enumerable.Empty<SwiftField>();
    }
}
