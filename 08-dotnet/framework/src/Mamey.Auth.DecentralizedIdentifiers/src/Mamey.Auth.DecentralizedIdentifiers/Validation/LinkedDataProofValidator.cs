using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Serialization;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;

namespace Mamey.Auth.DecentralizedIdentifiers.Validation;

/// <summary>
/// Validates Linked Data Proofs and the overall structure of Verifiable Credentials and Presentations.
/// </summary>
public class LinkedDataProofValidator
{
    private readonly IProofService _proofService;
    private readonly IDidResolver _didResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkedDataProofValidator"/> class.
    /// </summary>
    /// <param name="proofService">Proof service (signature validation, canonicalization).</param>
    /// <param name="didResolver">DID resolver (fetches issuer/holder keys).</param>
    public LinkedDataProofValidator(
        IProofService proofService,
        IDidResolver didResolver)
    {
        _proofService = proofService ?? throw new ArgumentNullException(nameof(proofService));
        _didResolver = didResolver ?? throw new ArgumentNullException(nameof(didResolver));
    }

    /// <summary>
    /// Validates a Verifiable Credential or Presentation, including signature(s) and structural compliance.
    /// </summary>
    /// <param name="jsonLd">The VC or VP as JSON-LD string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if all proofs and required structure are valid, else false.</returns>
    public async Task<bool> ValidateAsync(string jsonLd, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonLd)) throw new ArgumentNullException(nameof(jsonLd));

        using var doc = JsonDocument.Parse(jsonLd);
        var root = doc.RootElement;

        // Extract proof (can be array or object)
        var proofProp = LinkedDataProofSerializer.ExtractProof(root);

        if (proofProp == null)
            throw new ArgumentException("No 'proof' property found in credential or presentation.");

        if (proofProp.Value.ValueKind == JsonValueKind.Array)
        {
            foreach (var proof in proofProp.Value.EnumerateArray())
            {
                if (!await VerifySingleProofAsync(jsonLd, proof, cancellationToken))
                    return false;
            }

            return true;
        }
        else
        {
            return await VerifySingleProofAsync(jsonLd, proofProp.Value, cancellationToken);
        }
    }

    public static async Task<bool> Verify(
        JsonElement root,
        JsonElement proof,
        string verificationMethod,
        IProofService proofService,
        IDidResolver didResolver,
        CancellationToken cancellationToken = default)
    {
        // Get proof type and purpose (if available)
        string type = proof.GetProperty("type").GetString();
        string proofPurpose = proof.TryGetProperty("proofPurpose", out var pp) ? pp.GetString() : "assertionMethod";

        // Resolve DID for public key
        string did = verificationMethod.Split('#')[0];
        var didResult = await didResolver.ResolveAsync(did, cancellationToken);
        var didDoc = didResult.DidDocument;

        // Look up verification method by ID from DID Document
        var key = didDoc.VerificationMethods?.FirstOrDefault(vm => vm.Id == verificationMethod);
        if (key == null)
            throw new InvalidOperationException($"Verification method {verificationMethod} not found in DID document.");

// --- Add this type check ---
        if ((type == "Ed25519Signature2020" && key.Type != "Ed25519VerificationKey2020") ||
            (type == "EcdsaSecp256k1Signature2019" && key.Type != "EcdsaSecp256k1VerificationKey2019"))
        {
            throw new InvalidOperationException(
                $"Verification method type '{key.Type}' does not match proof type '{type}'.");
        }
// --- End type check ---

        byte[] publicKeyBytes = key.GetPublicKeyBytes();


        // Remove proof from JSON-LD for canonicalization
        var jsonLdWithoutProof = JsonUtils.RemoveProofProperty(root);
        string jsonLdStr = JsonSerializer.Serialize(jsonLdWithoutProof);

        // Call proof service for signature check
        return await proofService.VerifyProofAsync(
            jsonLdStr,
            proof,
            publicKeyBytes,
            type,
            proofPurpose,
            cancellationToken);
    }

    private async Task<bool> VerifySingleProofAsync(string jsonLd, JsonElement proof,
        CancellationToken cancellationToken)
    {
        // Get key details from verificationMethod
        string verificationMethod = proof.GetProperty("verificationMethod").GetString();
        string type = proof.GetProperty("type").GetString();
        string proofPurpose = proof.TryGetProperty("proofPurpose", out var pp) ? pp.GetString() : "assertionMethod";

        // Resolve DID for public key
        string did = verificationMethod.Split('#')[0];
        var didResult = await _didResolver.ResolveAsync(did, cancellationToken);
        var didDoc = didResult.DidDocument;

        // Look up verification method by ID from DID Document
        var key = didDoc.VerificationMethods?.FirstOrDefault(vm => vm.Id == verificationMethod);
        if (key == null)
            throw new InvalidOperationException($"Verification method {verificationMethod} not found in DID document.");

// --- Add this type check ---
        if ((type == "Ed25519Signature2020" && key.Type != "Ed25519VerificationKey2020") ||
            (type == "EcdsaSecp256k1Signature2019" && key.Type != "EcdsaSecp256k1VerificationKey2019"))
        {
            throw new InvalidOperationException(
                $"Verification method type '{key.Type}' does not match proof type '{type}'.");
        }
// --- End type check ---

        byte[] publicKeyBytes = key.GetPublicKeyBytes();


        // Remove proof from JSON-LD for canonicalization
        var jsonLdWithoutProof = JsonUtils.RemoveProofProperty(JsonDocument.Parse(jsonLd).RootElement);
        string jsonLdStr = JsonSerializer.Serialize(jsonLdWithoutProof);

        // Call proof service for signature check
        return await _proofService.VerifyProofAsync(
            jsonLdStr,
            proof,
            publicKeyBytes,
            type,
            proofPurpose,
            cancellationToken);
    }
}