using DnsClient;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Loads trusted DIDs from a DNS TXT record (e.g., "_trusted-dids.example.com").
/// </summary>
public class DnsTrustRegistry : ITrustRegistry
{
    private readonly string _dnsName;
    private readonly HashSet<string> _trustedIssuers = new(StringComparer.OrdinalIgnoreCase);

    public DnsTrustRegistry(string dnsName)
    {
        _dnsName = dnsName ?? throw new ArgumentNullException(nameof(dnsName));
    }

    public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
    public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var lookup = new LookupClient();
        var result = await lookup.QueryAsync(_dnsName, QueryType.TXT, cancellationToken: cancellationToken);
        var txtRecords = result.Answers.TxtRecords();
        _trustedIssuers.Clear();
        foreach (var record in txtRecords)
        {
            foreach (var txt in record.Text)
            {
                foreach (var did in txt.Split(new[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                    _trustedIssuers.Add(did.Trim());
            }
        }
    }
}