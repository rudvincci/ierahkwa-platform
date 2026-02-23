using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

public static class IonOperationBuilder
{
    /// <summary>
    /// Builds an ION "create" operation per ION protocol.
    /// </summary>
    public static object BuildCreateOperation(IonMethodOptions options)
    {
        // See: https://identity.foundation/ion/spec/#create-operation
        if (options.RecoveryPublicKeyJwk == null || options.UpdatePublicKeyJwk == null)
            throw new ArgumentException("Both RecoveryPublicKeyJwk and UpdatePublicKeyJwk are required.");

        // Build the DID Document
        var didDocument = new Dictionary<string, object>
        {
            ["publicKeys"] = options.PublicKeys ?? new List<IDictionary<string, object>>(),
            ["services"] = options.Services ?? new List<IDictionary<string, object>>()
        };

        // Create suffix data (commitment hashes)
        var recoveryCommitment = ComputeCommitment(options.RecoveryPublicKeyJwk);
        var updateCommitment = ComputeCommitment(options.UpdatePublicKeyJwk);

        var suffixData = new Dictionary<string, object>
        {
            ["recoveryCommitment"] = recoveryCommitment,
            ["deltaHash"] = ComputeDeltaHash(didDocument, updateCommitment)
        };

        // Delta object
        var delta = new Dictionary<string, object>
        {
            ["updateCommitment"] = updateCommitment,
            ["patches"] = new List<object>
            {
                new Dictionary<string, object>
                {
                    ["action"] = "replace",
                    ["document"] = didDocument
                }
            }
        };

        // Assemble create operation request
        var request = new Dictionary<string, object>
        {
            ["type"] = "create",
            ["suffixData"] = suffixData,
            ["delta"] = delta
        };

        // Add Bitcoin anchoring metadata if enabled
        if (options.BitcoinAnchoring?.Enabled == true)
        {
            request["bitcoinAnchoring"] = new Dictionary<string, object>
            {
                ["enabled"] = true,
                ["network"] = options.BitcoinAnchoring.Network,
                ["feeRate"] = options.BitcoinAnchoring.FeeRate,
                ["minConfirmations"] = options.BitcoinAnchoring.MinConfirmations
            };
        }

        // Optionally add more metadata if provided
        if (options.Metadata != null)
            request["metadata"] = options.Metadata;

        return request;
    }

    /// <summary>
    /// Builds an ION "update" operation per ION protocol.
    /// </summary>
    public static object BuildUpdateOperation(string did, IonUpdateOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.UpdatePrivateKeyJwk))
            throw new ArgumentException("UpdatePrivateKeyJwk is required.");

        // Sign delta hash (not shown; use ION tools to create JWS)
        var delta = new Dictionary<string, object>
        {
            ["patches"] = options.Patches ?? new List<IDictionary<string, object>>(),
            ["updateCommitment"] = options.NextUpdatePublicKeyJwk != null
                ? ComputeCommitment(options.NextUpdatePublicKeyJwk)
                : null
        };

        var deltaJson = JsonSerializer.Serialize(delta);

        // In production, you'd create a real JWS here with the update private key!
        var signedData = FakeSignWithJwk(options.UpdatePrivateKeyJwk, deltaJson);

        var request = new Dictionary<string, object>
        {
            ["type"] = "update",
            ["didSuffix"] = did.Split(':')[2], // did:ion:<suffix>
            ["revealValue"] = GetRevealValueFromJwk(options.UpdatePrivateKeyJwk),
            ["signedData"] = signedData,
            ["delta"] = delta
        };
        return request;
    }

    /// <summary>
    /// Builds an ION "deactivate" operation per ION protocol.
    /// </summary>
    public static object BuildDeactivateOperation(string did)
    {
        // In production, requires a signature from the recovery private key (as a JWS)
        return new Dictionary<string, object>
        {
            ["type"] = "deactivate",
            ["didSuffix"] = did.Split(':')[2],
            // ["revealValue"], ["signedData"], ... (add in real usage)
        };
    }

    // Below are placeholders — replace with ION’s own crypto routines and JWS logic in production!

    private static string ComputeCommitment(IDictionary<string, object> publicKeyJwk)
    {
        // This should be a multihash of the JWK (canonicalized JSON), e.g. SHA-256 then base64url-encode
        var json = JsonSerializer.Serialize(publicKeyJwk);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Base64UrlEncoder.Encode(hash);
    }

    private static string ComputeDeltaHash(object deltaDoc, string updateCommitment)
    {
        // Canonicalize delta, hash, then encode; simplified here for demonstration
        var json = JsonSerializer.Serialize(deltaDoc);
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(json + updateCommitment));
        return Base64UrlEncoder.Encode(hash);
    }

    private static string GetRevealValueFromJwk(string jwk)
    {
        // In ION, revealValue is usually hash(commitment key JWK, etc); simplified for demo
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(jwk));
        return Base64UrlEncoder.Encode(hash);
    }

    private static string FakeSignWithJwk(string jwk, string payload)
    {
        // Placeholder — replace with real JWS signing!
        return Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(payload));
    }
    private static string SignWithJwk(string payloadJson, string privateKeyJwkJson)
    {
        return IonExtensions.JwsSignWithNode(payloadJson, privateKeyJwkJson);
    }
    
    
}
