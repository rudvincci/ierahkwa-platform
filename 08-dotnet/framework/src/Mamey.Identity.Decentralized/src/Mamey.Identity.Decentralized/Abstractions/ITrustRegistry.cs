namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Common trust registry interface, supporting add/remove, federated lookup, and remote sync.
/// </summary>
public interface ITrustRegistry
{
    void AddTrustedIssuer(string did);
    void RemoveTrustedIssuer(string did);
    bool IsTrusted(string did);

    IEnumerable<string> AllTrusted();

    // Optional: async update/poll/load pattern
    Task RefreshAsync(CancellationToken cancellationToken = default);
}