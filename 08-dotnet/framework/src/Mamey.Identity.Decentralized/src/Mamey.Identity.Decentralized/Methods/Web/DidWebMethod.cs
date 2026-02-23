using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Methods.MethodBase;

namespace Mamey.Identity.Decentralized.Methods.Web;

/// <summary>
/// Implements the did:web method with Linked Data Proofs.
/// </summary>
public class DidWebMethod : DidMethodBase
{
    public override string Name => "web";
    private readonly HttpClient _httpClient;
    private readonly IProofService _proofService;

    public DidWebMethod(HttpClient httpClient, IProofService proofService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _proofService = proofService ?? throw new ArgumentNullException(nameof(proofService));
    }

    /// <inheritdoc/>
    public override async Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);
        var url = ToDidWebUrl(did);

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var json = await new StreamReader(stream).ReadToEndAsync();
        var doc = JsonSerializer.Deserialize<DidDocumentWithProof>(json);

        // Verify proof (if present)
        if (doc.Proof != null)
        {
            var docJson = JsonSerializer.Serialize(doc, new JsonSerializerOptions { IgnoreNullValues = true });
            bool isValid = await _proofService.VerifyProofAsync(
                docJson,
                doc.Proof,
                GetPublicKeyFromVerificationMethod(doc),
                doc.Proof.Type,
                doc.Proof.ProofPurpose,
                cancellationToken);

            if (!isValid)
                throw new InvalidOperationException("DID Document signature/proof is invalid.");
        }
        return doc.ToDidDocument();
    }

    public override async Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
    {
        if (options is not WebMethodOptions webOptions)
            throw new ArgumentException("Invalid options for did:web creation.");

        var url = ToDidWebUrl(webOptions);
        var didDocument = BuildDidDocument(webOptions);

        // Sign the DID Document with Linked Data Proofs
        var docJson = JsonSerializer.Serialize(didDocument);
        var proof = await _proofService.CreateProofAsync(
            docJson,
            $"{didDocument.Id}#key-1",
            webOptions.PrivateKey,
            "assertionMethod",
            webOptions.KeyType ?? "Ed25519");

        var docWithProof = new DidDocumentWithProof(didDocument, proof);
        var json = JsonSerializer.Serialize(docWithProof, new JsonSerializerOptions { WriteIndented = true });

        var response = await _httpClient.PutAsync(url, new StringContent(json), cancellationToken);
        response.EnsureSuccessStatusCode();

        // Confirm by re-fetching
        var did = BuildDidWebIdentifier(webOptions);
        return await ResolveAsync(did, cancellationToken);
    }

    public override async Task<IDidDocument> UpdateAsync(string did, object updateRequest, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        if (updateRequest is not DidDocument updatedDoc)
            throw new ArgumentException("updateRequest must be a DidDocument instance.");

        var url = ToDidWebUrl(did);

        // (Optionally re-sign document after update)
        // You may want to require a key or proof in your update request/options
        var json = JsonSerializer.Serialize(updatedDoc, new JsonSerializerOptions { WriteIndented = true });

        var response = await _httpClient.PutAsync(url, new StringContent(json), cancellationToken);
        response.EnsureSuccessStatusCode();

        return await ResolveAsync(did, cancellationToken);
    }

    public override async Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        var url = ToDidWebUrl(did);
        var response = await _httpClient.DeleteAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NotFound)
        {
            throw new Exception($"Failed to deactivate did:web: {did} at {url}. Status: {response.StatusCode}");
        }
    }

    private static DidDocument BuildDidDocument(WebMethodOptions options)
    {
        var methodSpecificId = options.Domain +
                               (options.PathSegments is { Length: > 0 }
                                   ? ":" + string.Join(":", options.PathSegments)
                                   : "");

        var did = $"did:web:{methodSpecificId}";
        var controller = string.IsNullOrWhiteSpace(options.Controller) ? did : options.Controller;

        var verificationMethods = new[]
        {
            new VerificationMethod(
                id: $"{did}#key-1",
                type: "JsonWebKey2020",
                controller: controller,
                publicKeyJwk: options.PublicKey != null
                    ? new Dictionary<string, object>
                    {
                        { "kty", "EC" },
                        { "crv", "P-256" }, // Or as required
                        { "x", Convert.ToBase64String(options.PublicKey) }
                    }
                    : null)
        }.ToList();

        return new DidDocument(
            context: new[] { "https://www.w3.org/ns/did/v1" },
            id: did,
            controller: new[] { controller },
            verificationMethods: verificationMethods,
            authentication: new[] { $"{did}#key-1" },
            assertionMethod: new[] { $"{did}#key-1" },
            keyAgreement: null,
            capabilityDelegation: null,
            capabilityInvocation: null,
            serviceEndpoints: options.ServiceEndpoints?.OfType<IDidServiceEndpoint>().ToList(),
            additionalProperties: options.Metadata as IDictionary<string, object>
        );
    }

    private static string BuildDidWebIdentifier(WebMethodOptions options)
    {
        var methodSpecificId = options.Domain +
                               (options.PathSegments is { Length: > 0 }
                                   ? ":" + string.Join(":", options.PathSegments)
                                   : "");
        return $"did:web:{methodSpecificId}";
    }

    private static string ToDidWebUrl(string did)
    {
        var didObj = new Did(did);
        var segments = didObj.MethodSpecificId.Split(':');
        var host = segments[0];
        var path = segments.Length > 1
            ? "/" + string.Join("/", segments.Skip(1))
            : string.Empty;
        var url = $"https://{host}{(path == string.Empty ? "/.well-known" : path)}/did.json";
        return url;
    }

    private static string ToDidWebUrl(WebMethodOptions options)
    {
        var host = options.Domain;
        var path = options.PathSegments is { Length: > 0 }
            ? "/" + string.Join("/", options.PathSegments)
            : string.Empty;
        var url = $"https://{host}{(path == string.Empty ? "/.well-known" : path)}/did.json";
        return url;
    }

    /// <summary>
    /// Retrieves the public key bytes from the first verification method of a DID Document (for proof verification).
    /// </summary>
    private static byte[] GetPublicKeyFromVerificationMethod(DidDocument doc)
    {
        if (doc.VerificationMethods == null || doc.VerificationMethods.Count == 0)
            throw new InvalidOperationException("No verification methods found in DID Document.");

        var method = doc.VerificationMethods.First();
        // Support JWK, Base58, Multibase etc.
        if (method.PublicKeyJwk != null && method.PublicKeyJwk.TryGetValue("x", out var x))
            return Convert.FromBase64String(x.ToString());
        if (!string.IsNullOrWhiteSpace(method.PublicKeyBase58))
            return Crypto.Base58Util.Decode(method.PublicKeyBase58);
        if (!string.IsNullOrWhiteSpace(method.PublicKeyMultibase))
            return Crypto.MultibaseUtil.Decode(method.PublicKeyMultibase);

        throw new InvalidOperationException("Public key extraction failed (no supported encoding found).");
    }
}