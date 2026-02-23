using System.Text.Json;

namespace MameyNode.UI.Services;

public class AuthorityRegistryStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _path;

    public AuthorityRegistryStore(IWebHostEnvironment env)
    {
        // Keep dev artifacts inside the UI project (not treated as complete/production).
        var dataDir = Path.Combine(env.ContentRootPath, ".data", "authority-ui");
        Directory.CreateDirectory(dataDir);
        _path = Path.Combine(dataDir, "registry.json");
    }

    public async Task<AuthorityRegistry> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_path))
        {
            return new AuthorityRegistry();
        }

        var json = await File.ReadAllTextAsync(_path, cancellationToken);
        return JsonSerializer.Deserialize<AuthorityRegistry>(json, JsonOptions) ?? new AuthorityRegistry();
    }

    public async Task SaveAsync(AuthorityRegistry registry, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(registry, JsonOptions);
        await File.WriteAllTextAsync(_path, json, cancellationToken);
    }
}

public class AuthorityRegistry
{
    public List<GovernmentAuthorityRecord> Governments { get; set; } = new();
    public List<BankInstitutionRecord> Banks { get; set; } = new();
    public List<GovOperatorCredentialRecord> GovOperatorCredentials { get; set; } = new();
    public List<BankCharterCredentialRecord> BankCharterCredentials { get; set; } = new();
}

public record GovernmentAuthorityRecord(
    string Did,
    string Jurisdiction,
    string WalletKeyId,
    string WalletPublicKey,
    DateTimeOffset CreatedAt);

public record BankInstitutionRecord(
    string Did,
    string Jurisdiction,
    string WalletKeyId,
    string WalletPublicKey,
    DateTimeOffset CreatedAt);

public record GovOperatorCredentialRecord(
    string Id,
    string IssuerDid,
    string SubjectDid,
    DateTimeOffset IssuedAt,
    DateTimeOffset? ExpiresAt,
    List<string> Capabilities,
    string Jurisdiction,
    string SignatureHex,
    string PublicKeyId);

public record BankCharterCredentialRecord(
    string Id,
    string IssuerDid,
    string SubjectDid,
    DateTimeOffset IssuedAt,
    DateTimeOffset? ExpiresAt,
    List<string> Capabilities,
    string CharterNumber,
    string Jurisdiction,
    string SignatureHex,
    string PublicKeyId);

