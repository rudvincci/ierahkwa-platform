using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Aggregates multiple trust registries and delegates lookups/refresh.
/// </summary>
public class MultiSourceTrustRegistry : ITrustRegistry
{
    private readonly IList<ITrustRegistry> _registries;

    /// <summary>
    /// Initializes a new MultiSourceTrustRegistry with other registries.
    /// </summary>
    public MultiSourceTrustRegistry(IEnumerable<ITrustRegistry> registries)
    {
        _registries = registries?.ToList() ?? new List<ITrustRegistry>();
    }

    /// <inheritdoc/>
    public void AddTrustedIssuer(string did)
    {
        foreach (var reg in _registries)
            reg.AddTrustedIssuer(did);
    }

    /// <inheritdoc/>
    public void RemoveTrustedIssuer(string did)
    {
        foreach (var reg in _registries)
            reg.RemoveTrustedIssuer(did);
    }

    /// <inheritdoc/>
    public bool IsTrusted(string did) => _registries.Any(reg => reg.IsTrusted(did));

    /// <inheritdoc/>
    public IEnumerable<string> AllTrusted() => _registries.SelectMany(r => r.AllTrusted()).Distinct();

    /// <inheritdoc/>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        foreach (var reg in _registries)
            await reg.RefreshAsync(cancellationToken);
    }
}