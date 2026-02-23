using JsonLD.Core;
using Newtonsoft.Json.Linq;

namespace Mamey.Identity.Decentralized.Serialization;

/// <summary>
/// Provides JSON-LD expansion, compaction, and context validation.
/// </summary>
public static class JsonLdSerializer
{
    /// <summary>
    /// Expands the JSON-LD input, resolving context.
    /// </summary>
    public static string? Expand(string jsonLd, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonLd)) throw new ArgumentNullException(nameof(jsonLd));
        var input = JToken.Parse(jsonLd);
        var expanded = JsonLdProcessor.Expand(input, new JsonLdOptions());
        return expanded?.ToString(Newtonsoft.Json.Formatting.None);
    }

    /// <summary>
    /// Compacts a JSON-LD input with the given context.
    /// </summary>
    public static string? Compact(string jsonLd, string context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonLd)) throw new ArgumentNullException(nameof(jsonLd));
        if (string.IsNullOrWhiteSpace(context)) throw new ArgumentNullException(nameof(context));
        var input = JToken.Parse(jsonLd);
        var ctx = JToken.Parse(context);
        var compacted = JsonLdProcessor.Compact(input, ctx, new JsonLdOptions());
        return compacted?.ToString(Newtonsoft.Json.Formatting.None);
    }

    /// <summary>
    /// Validates that the JSON-LD input conforms to the required context.
    /// </summary>
    public static bool Validate(string jsonLd, string requiredContext, CancellationToken cancellationToken = default)
    {
        var expanded = Expand(jsonLd, cancellationToken);
        return expanded.Contains(requiredContext, StringComparison.OrdinalIgnoreCase);
    }
}