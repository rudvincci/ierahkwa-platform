using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey SICB Treasury Key Custodies", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var keys = new Dictionary<string, KeyCustody>();
var vaults = new Dictionary<string, Vault>
{
    ["MAIN_VAULT"] = new Vault { VaultId = "MAIN_VAULT", Name = "Main Treasury Vault", Type = VaultType.Hot, Status = VaultStatus.Active },
    ["COLD_STORAGE"] = new Vault { VaultId = "COLD_STORAGE", Name = "Cold Storage Vault", Type = VaultType.Cold, Status = VaultStatus.Active },
    ["HSM_VAULT"] = new Vault { VaultId = "HSM_VAULT", Name = "HSM Protected Vault", Type = VaultType.HSM, Status = VaultStatus.Active }
};
long counter = 1000000;

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.TreasuryKeyCustodies" }));

app.MapGet("/api/v1/vaults", () => Results.Ok(new { vaults = vaults.Values }));

app.MapPost("/api/v1/keys/generate", (GenerateKeyRequest req) =>
{
    var id = Interlocked.Increment(ref counter);
    var keyPair = GenerateKeyPair(req.Algorithm);
    
    var custody = new KeyCustody
    {
        KeyId = $"KEY-{id}",
        Algorithm = req.Algorithm,
        Purpose = req.Purpose,
        VaultId = req.VaultId,
        PublicKey = keyPair.PublicKey,
        PublicKeyHash = ComputeHash(keyPair.PublicKey),
        Status = KeyStatus.Active,
        CreatedBy = req.CreatedBy,
        RequiredSignatures = req.RequiredSignatures,
        CreatedAt = DateTime.UtcNow
    };
    
    // Store private key encrypted (simulated)
    custody.EncryptedPrivateKey = EncryptKey(keyPair.PrivateKey, req.VaultId);
    
    keys[custody.KeyId] = custody;
    
    return Results.Ok(new { 
        success = true, 
        key = new { custody.KeyId, custody.PublicKey, custody.PublicKeyHash, custody.Purpose }
    });
});

app.MapGet("/api/v1/keys/{keyId}", (string keyId) =>
{
    if (!keys.TryGetValue(keyId, out var key))
        return Results.NotFound();
    
    return Results.Ok(new { key = new { key.KeyId, key.PublicKey, key.Purpose, key.Status, key.Algorithm } });
});

app.MapPost("/api/v1/keys/{keyId}/sign", (string keyId, SignRequest req) =>
{
    if (!keys.TryGetValue(keyId, out var key))
        return Results.NotFound();
    
    if (key.Status != KeyStatus.Active)
        return Results.BadRequest(new { error = "Key is not active" });
    
    // Simulate signing
    var signature = Convert.ToBase64String(SHA256.HashData(Convert.FromBase64String(req.Data + key.PublicKeyHash)));
    
    return Results.Ok(new { 
        success = true, 
        signature,
        keyId,
        algorithm = key.Algorithm
    });
});

app.MapPost("/api/v1/keys/{keyId}/rotate", (string keyId, RotateKeyRequest req) =>
{
    if (!keys.TryGetValue(keyId, out var oldKey))
        return Results.NotFound();
    
    // Deactivate old key
    oldKey.Status = KeyStatus.Rotated;
    oldKey.RotatedAt = DateTime.UtcNow;
    
    // Generate new key
    var id = Interlocked.Increment(ref counter);
    var keyPair = GenerateKeyPair(oldKey.Algorithm);
    
    var newKey = new KeyCustody
    {
        KeyId = $"KEY-{id}",
        Algorithm = oldKey.Algorithm,
        Purpose = oldKey.Purpose,
        VaultId = oldKey.VaultId,
        PublicKey = keyPair.PublicKey,
        PublicKeyHash = ComputeHash(keyPair.PublicKey),
        Status = KeyStatus.Active,
        CreatedBy = req.RotatedBy,
        PreviousKeyId = oldKey.KeyId,
        CreatedAt = DateTime.UtcNow
    };
    
    keys[newKey.KeyId] = newKey;
    
    return Results.Ok(new { success = true, oldKeyId = oldKey.KeyId, newKeyId = newKey.KeyId });
});

app.MapPost("/api/v1/keys/{keyId}/revoke", (string keyId, RevokeKeyRequest req) =>
{
    if (!keys.TryGetValue(keyId, out var key))
        return Results.NotFound();
    
    key.Status = KeyStatus.Revoked;
    key.RevokedAt = DateTime.UtcNow;
    key.RevokedBy = req.RevokedBy;
    key.RevocationReason = req.Reason;
    
    return Results.Ok(new { success = true });
});

(string PublicKey, string PrivateKey) GenerateKeyPair(string algorithm)
{
    var bytes = new byte[32];
    RandomNumberGenerator.Fill(bytes);
    return (
        Convert.ToBase64String(bytes),
        Convert.ToBase64String(SHA256.HashData(bytes))
    );
}

string ComputeHash(string data) => Convert.ToHexString(SHA256.HashData(Convert.FromBase64String(data))).ToLower();

string EncryptKey(string key, string vaultId) => Convert.ToBase64String(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key + vaultId)));

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║      MAMEY SICB TREASURY KEY CUSTODIES v1.0.0                ║");
Console.WriteLine("║      Secure Key Management & HSM Integration                 ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record GenerateKeyRequest(string Algorithm, string Purpose, string VaultId, string CreatedBy, int RequiredSignatures = 1);
record SignRequest(string Data);
record RotateKeyRequest(string RotatedBy);
record RevokeKeyRequest(string RevokedBy, string Reason);

class KeyCustody
{
    public string KeyId { get; set; } = "";
    public string Algorithm { get; set; } = "";
    public string Purpose { get; set; } = "";
    public string VaultId { get; set; } = "";
    public string PublicKey { get; set; } = "";
    public string PublicKeyHash { get; set; } = "";
    public string? EncryptedPrivateKey { get; set; }
    public KeyStatus Status { get; set; }
    public string? CreatedBy { get; set; }
    public int RequiredSignatures { get; set; }
    public string? PreviousKeyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RotatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedBy { get; set; }
    public string? RevocationReason { get; set; }
}

class Vault
{
    public string VaultId { get; set; } = "";
    public string Name { get; set; } = "";
    public VaultType Type { get; set; }
    public VaultStatus Status { get; set; }
}

enum KeyStatus { Active, Rotated, Revoked, Expired }
enum VaultType { Hot, Cold, HSM }
enum VaultStatus { Active, Locked, Maintenance }
