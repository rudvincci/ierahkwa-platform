using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;

namespace Mamey.Identity.Decentralized.Trust;

/// <summary>
/// Loads trusted DIDs from a JSON file (one array property: [ "did:...", ... ]).
/// </summary>
public class FileTrustRegistry : ITrustRegistry
{
    private readonly string _filePath;
    private readonly HashSet<string> _trustedIssuers = new(StringComparer.OrdinalIgnoreCase);

    public FileTrustRegistry(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        LoadFile();
    }

    public void AddTrustedIssuer(string did)
    {
        _trustedIssuers.Add(did);
        SaveFile();
    }

    public void RemoveTrustedIssuer(string did)
    {
        _trustedIssuers.Remove(did);
        SaveFile();
    }

    public bool IsTrusted(string did) => _trustedIssuers.Contains(did);

    public IEnumerable<string> AllTrusted() => _trustedIssuers;

    public Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        LoadFile();
        return Task.CompletedTask;
    }

    private void LoadFile()
    {
        if (!File.Exists(_filePath)) return;
        var json = File.ReadAllText(_filePath);
        var dids = JsonSerializer.Deserialize<List<string>>(json);
        _trustedIssuers.Clear();
        if (dids != null)
            foreach (var did in dids)
                _trustedIssuers.Add(did);
    }

    private void SaveFile()
    {
        var json = JsonSerializer.Serialize(_trustedIssuers);
        File.WriteAllText(_filePath, json);
    }
}