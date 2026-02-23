using System.Text.RegularExpressions;

namespace Mamey.Identity.Decentralized.Utilities;

/// <summary>
/// Utility functions for working with DID strings and basic parsing.
/// </summary>
public static class DidUtils
{
    private static readonly Regex DidRegex = new(@"^did:([a-z0-9]+):([a-zA-Z0-9.\-_:]+)$", RegexOptions.Compiled);

    /// <summary>
    /// Validates whether a string is a conformant DID.
    /// </summary>
    public static bool IsValidDid(string did)
    {
        if (string.IsNullOrWhiteSpace(did)) return false;
        return DidRegex.IsMatch(did);
    }

    /// <summary>
    /// Extracts the method from a DID string.
    /// </summary>
    public static string GetMethod(string did)
    {
        var match = DidRegex.Match(did ?? "");
        return match.Success ? match.Groups[1].Value : null;
    }

    /// <summary>
    /// Returns the method-specific id.
    /// </summary>
    public static string GetMethodSpecificId(string did)
    {
        var match = DidRegex.Match(did ?? "");
        return match.Success ? match.Groups[2].Value : null;
    }
}

/// <summary>
/// Represents a single audit event for DID/VC/VP actions.
/// </summary>
public class AuditEvent
{
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// "DID", "VC", or "VP"
    /// </summary>
    public string ObjectType { get; set; }

    /// <summary>
    /// The unique ID (DID, VC ID, etc).
    /// </summary>
    public string ObjectId { get; set; }

    /// <summary>
    /// The event type (e.g. "created", "updated", "revoked", "verified").
    /// </summary>
    public string EventType { get; set; }

    /// <summary>
    /// Who/what triggered this event (DID, user, API, etc).
    /// </summary>
    public string Actor { get; set; }

    /// <summary>
    /// (Optional) Outcome or result (e.g. "success", "failure", "invalid", etc).
    /// </summary>
    public string Outcome { get; set; }

    /// <summary>
    /// Arbitrary extra data (IP, signature, hash, etc).
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}