namespace Mamey.Identity.Decentralized.Trust;

/// <summary>
/// Represents a federated list of trusted issuers (e.g., loaded from another government, industry, or ecosystem).
/// </summary>
public class TrustedIssuerList
{
    public string ListId { get; }
    public string Source { get; }
    public IReadOnlyList<string> Issuers { get; }

    public TrustedIssuerList(string listId, string source, IEnumerable<string> issuers)
    {
        ListId = listId ?? throw new ArgumentNullException(nameof(listId));
        Source = source;
        Issuers = new List<string>(issuers ?? Array.Empty<string>());
    }

    public bool IsTrusted(string did) => Issuers.Contains(did, StringComparer.OrdinalIgnoreCase);
}