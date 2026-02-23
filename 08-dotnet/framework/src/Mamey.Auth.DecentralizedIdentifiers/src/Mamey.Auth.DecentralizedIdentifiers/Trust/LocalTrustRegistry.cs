using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Loads trusted DIDs from a static in-memory set (optionally from config).
/// </summary>
public class LocalTrustRegistry : ITrustRegistry
{
    private readonly HashSet<string> _trustedIssuers;

    public LocalTrustRegistry(IEnumerable<string> initialTrusted = null)
    {
        _trustedIssuers =
            new HashSet<string>(initialTrusted ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);
    }

    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        // Nothing to do for local registry
        return Task.CompletedTask;
    }
}