using Mamey.Auth.DecentralizedIdentifiers.Abstractions;

namespace Mamey.Auth.DecentralizedIdentifiers.Trust;

/// <summary>
/// Local, in-memory trust registry for testing or static trust anchors.
/// </summary>
public class TrustRegistry : ITrustRegistry
{
    protected readonly HashSet<string> _trustedIssuers = new(StringComparer.OrdinalIgnoreCase);
    protected readonly List<TrustedIssuerList> _trustedLists = new();

    public virtual void AddTrustedIssuer(string did)
    {
        if (string.IsNullOrWhiteSpace(did))
            throw new ArgumentNullException(nameof(did));
        _trustedIssuers.Add(did);
    }

    public virtual void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);

    public virtual void AddTrustedIssuerList(TrustedIssuerList list)
    {
        if (list == null) throw new ArgumentNullException(nameof(list));
        _trustedLists.Add(list);
    }

    public virtual bool IsTrusted(string did)
    {
        if (_trustedIssuers.Contains(did))
            return true;
        return _trustedLists.Any(list => list.IsTrusted(did));
    }

    public virtual IEnumerable<string> AllTrusted()
    {
        foreach (var did in _trustedIssuers) yield return did;
        foreach (var list in _trustedLists)
        foreach (var did in list.Issuers)
            yield return did;
    }

    public virtual Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        // No-op for local; override in subclasses
        return Task.CompletedTask;
    }
}

// /// <summary>
// /// Loads trusted DIDs from an ENS (Ethereum Name Service) text record.
// /// </summary>
// public class ENSTrustRegistry : ITrustRegistry
// {
//     private readonly Web3 _web3;
//     private readonly string _ensDomain;
//     private readonly HashSet<string> _trustedIssuers = new(StringComparer.OrdinalIgnoreCase);
//
//     public ENSTrustRegistry(Web3 web3, string ensDomain)
//     {
//         _web3 = web3 ?? throw new ArgumentNullException(nameof(web3));
//         _ensDomain = ensDomain ?? throw new ArgumentNullException(nameof(ensDomain));
//     }
//
//     public void AddTrustedIssuer(string did) => _trustedIssuers.Add(did);
//     public void RemoveTrustedIssuer(string did) => _trustedIssuers.Remove(did);
//     public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
//     public IEnumerable<string> AllTrusted() => _trustedIssuers;
//
//     public async Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         // Get ENS resolver for the domain
//         var resolver = new ENSResolverService(_web3, _ensDomain);
//         // Retrieve the "trustedDids" text record
//         string txt = await resolver.GetTextAsync("trustedDids");
//         if (!string.IsNullOrWhiteSpace(txt))
//         {
//             var dids = txt.Split(new[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
//             _trustedIssuers.Clear();
//             foreach (var did in dids)
//                 _trustedIssuers.Add(did.Trim());
//         }
//     }
// }



// public class SAMLTrustRegistry : ITrustRegistry
// {
//     private readonly string _metadataUrl;
//     private readonly HashSet<string> _trustedEntityIds = new();
//     private readonly HttpClient _client;
//
//     public SAMLTrustRegistry(string metadataUrl, HttpClient client)
//     {
//         _metadataUrl = metadataUrl;
//         _client = client;
//     }
//
//     public void AddTrustedIssuer(string entityId) => _trustedEntityIds.Add(entityId);
//     public void RemoveTrustedIssuer(string entityId) => _trustedEntityIds.Remove(entityId);
//     public bool IsTrusted(string entityId) => _trustedEntityIds.Contains(entityId);
//     public IEnumerable<string> AllTrusted() => _trustedEntityIds;
//
//     public async Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         var xml = await _client.GetStringAsync(_metadataUrl, cancellationToken);
//         var doc = XDocument.Parse(xml);
//         foreach (var entity in doc.Descendants("{urn:oasis:names:tc:SAML:2.0:metadata}EntityDescriptor"))
//         {
//             var entityId = entity.Attribute("entityID")?.Value;
//             if (!string.IsNullOrWhiteSpace(entityId))
//                 _trustedEntityIds.Add(entityId);
//         }
//     }
// }
//
// public class X509TrustRegistry : ITrustRegistry
// {
//     private readonly HashSet<string> _trustedThumbprints = new();
//     private readonly StoreName _storeName;
//     private readonly StoreLocation _storeLocation;
//
//     public X509TrustRegistry(StoreName storeName = StoreName.Root,
//         StoreLocation storeLocation = StoreLocation.CurrentUser)
//     {
//         _storeName = storeName;
//         _storeLocation = storeLocation;
//     }
//
//     public void AddTrustedIssuer(string thumbprint) => _trustedThumbprints.Add(thumbprint);
//     public void RemoveTrustedIssuer(string thumbprint) => _trustedThumbprints.Remove(thumbprint);
//     public bool IsTrusted(string thumbprint) => _trustedThumbprints.Contains(thumbprint);
//     public IEnumerable<string> AllTrusted() => _trustedThumbprints;
//
//     public Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         using var store = new X509Store(_storeName, _storeLocation);
//         store.Open(OpenFlags.ReadOnly);
//         _trustedThumbprints.Clear();
//         foreach (var cert in store.Certificates)
//             _trustedThumbprints.Add(cert.Thumbprint);
//         return Task.CompletedTask;
//     }
// }
//
// public class SQLTrustRegistry : ITrustRegistry
// {
//     private readonly string _connectionString;
//     private readonly HashSet<string> _trustedIssuers = new();
//
//     public SQLTrustRegistry(string connectionString)
//     {
//         _connectionString = connectionString;
//     }
//
//     public void AddTrustedIssuer(string did)
//     {
//         _trustedIssuers.Add(did);
//         // Insert into DB (production: parameterized!)
//         using var conn = new SqlConnection(_connectionString);
//         conn.Open();
//         using var cmd = conn.CreateCommand();
//         cmd.CommandText = "INSERT INTO TrustedIssuers (Did) VALUES (@did)";
//         cmd.Parameters.AddWithValue("@did", did);
//         cmd.ExecuteNonQuery();
//     }
//
//     public void RemoveTrustedIssuer(string did)
//     {
//         _trustedIssuers.Remove(did);
//         using var conn = new SqlConnection(_connectionString);
//         conn.Open();
//         using var cmd = conn.CreateCommand();
//         cmd.CommandText = "DELETE FROM TrustedIssuers WHERE Did = @did";
//         cmd.Parameters.AddWithValue("@did", did);
//         cmd.ExecuteNonQuery();
//     }
//
//     public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
//
//     public IEnumerable<string> AllTrusted() => _trustedIssuers;
//
//     public async Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         _trustedIssuers.Clear();
//         using var conn = new SqlConnection(_connectionString);
//         await conn.OpenAsync(cancellationToken);
//         using var cmd = conn.CreateCommand();
//         cmd.CommandText = "SELECT Did FROM TrustedIssuers";
//         using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
//         while (await reader.ReadAsync(cancellationToken))
//             _trustedIssuers.Add(reader.GetString(0));
//     }
// }
//
// public class MongoTrustRegistry : ITrustRegistry
// {
//     private readonly IMongoCollection<TrustedIssuerDoc> _collection;
//     private readonly HashSet<string> _trustedIssuers = new();
//
//     public class TrustedIssuerDoc
//     {
//         public string Id { get; set; }
//         public string Did { get; set; }
//     }
//
//     public MongoTrustRegistry(IMongoCollection<TrustedIssuerDoc> collection)
//     {
//         _collection = collection;
//     }
//
//     public void AddTrustedIssuer(string did)
//     {
//         _trustedIssuers.Add(did);
//         _collection.InsertOne(new TrustedIssuerDoc { Did = did });
//     }
//
//     public void RemoveTrustedIssuer(string did)
//     {
//         _trustedIssuers.Remove(did);
//         _collection.DeleteOne(x => x.Did == did);
//     }
//
//     public bool IsTrusted(string did) => _trustedIssuers.Contains(did);
//
//     public IEnumerable<string> AllTrusted() => _trustedIssuers;
//
//     public async Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         _trustedIssuers.Clear();
//         var docs = await _collection.Find(_ => true).ToListAsync(cancellationToken);
//         foreach (var doc in docs) _trustedIssuers.Add(doc.Did);
//     }
// }
//
// /// <summary>
// /// Loads trusted issuer DIDs from a JSON file stored in AWS S3.
// /// </summary>
// public class S3TrustRegistry : ITrustRegistry
// {
//     private readonly string _bucket;
//     private readonly string _key;
//     private readonly IAmazonS3 _s3;
//     private readonly HashSet<string> _trustedDids = new();
//
//     /// <summary>
//     /// Initializes a new S3 trust registry.
//     /// </summary>
//     /// <param name="bucket">S3 bucket name.</param>
//     /// <param name="key">Object key (file path in S3).</param>
//     /// <param name="s3">Amazon S3 client (auth as needed).</param>
//     public S3TrustRegistry(string bucket, string key, IAmazonS3 s3)
//     {
//         _bucket = bucket;
//         _key = key;
//         _s3 = s3;
//     }
//
//     /// <inheritdoc/>
//     public void AddTrustedIssuer(string did) => _trustedDids.Add(did);
//
//     /// <inheritdoc/>
//     public void RemoveTrustedIssuer(string did) => _trustedDids.Remove(did);
//
//     /// <inheritdoc/>
//     public bool IsTrusted(string did) => _trustedDids.Contains(did);
//
//     /// <inheritdoc/>
//     public IEnumerable<string> AllTrusted() => _trustedDids;
//
//     /// <inheritdoc/>
//     public async Task RefreshAsync(CancellationToken cancellationToken = default)
//     {
//         var resp = await _s3.GetObjectAsync(_bucket, _key, cancellationToken);
//         using var stream = resp.ResponseStream;
//         var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
//
//         _trustedDids.Clear();
//         if (doc.RootElement.ValueKind == JsonValueKind.Array)
//             foreach (var did in doc.RootElement.EnumerateArray())
//                 _trustedDids.Add(did.GetString());
//     }
// }