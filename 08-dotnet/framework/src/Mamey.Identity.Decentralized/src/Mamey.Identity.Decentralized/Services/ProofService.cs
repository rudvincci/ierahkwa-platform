using System.Text;
using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Microsoft.Extensions.Logging;
using Nethereum.Signer;
using NSec.Cryptography;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Mamey.Identity.Decentralized.Services;

public class ProofService : IProofService
{
    private readonly ILogger<CredentialStatusService> _logger;
    private readonly HttpClient _httpClient;

    public ProofService(ILogger<CredentialStatusService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<object> CreateProofAsync(
        string jsonLd,
        string verificationMethodId,
        byte[] privateKey,
        string proofPurpose,
        string type,
        string created = null,
        CancellationToken cancellationToken = default)
    {
        string canon = await CanonicalizeAsync(jsonLd, cancellationToken);

        byte[] signatureBytes;
        if (type == "Ed25519Signature2020")
        {
            using var key = Key.Import(SignatureAlgorithm.Ed25519, privateKey, KeyBlobFormat.RawPrivateKey);
            signatureBytes = SignatureAlgorithm.Ed25519.Sign(key, Encoding.UTF8.GetBytes(canon));
        }
        else if (type == "EcdsaSecp256k1Signature2019")
        {
            var ethKey = new EthECKey(privateKey, true);
            var signer = new EthereumMessageSigner();
            var msgHash = signer.HashPrefixedMessage(Encoding.UTF8.GetBytes(canon));
            var signature = signer.Sign(msgHash, ethKey);
            signatureBytes = Encoding.UTF8.GetBytes(signature); // (signature is hex string)
        }
        else
        {
            throw new NotSupportedException($"Signature type '{type}' is not supported.");
        }

        return new
        {
            type,
            created = created ?? DateTime.UtcNow.ToString("o"),
            verificationMethod = verificationMethodId,
            proofPurpose,
            proofValue = Convert.ToBase64String(signatureBytes)
        };
    }

    public async Task<bool> VerifyProofAsync(
        string jsonLd,
        object proofObj,
        byte[] publicKey,
        string type,
        string proofPurpose,
        CancellationToken cancellationToken = default)
    {
        string canon = await CanonicalizeAsync(jsonLd, cancellationToken);
        JsonElement proof = JsonSerializer.SerializeToElement(proofObj);

        if (!proof.TryGetProperty("type", out var typeElem) || typeElem.GetString() != type)
            return false;
        if (!proof.TryGetProperty("proofPurpose", out var purposeElem) || purposeElem.GetString() != proofPurpose)
            return false;
        if (!proof.TryGetProperty("proofValue", out var valueElem))
            return false;

        if (type == "Ed25519Signature2020")
        {
            // proofValue is base64-encoded Ed25519 signature bytes
            var signature = Convert.FromBase64String(valueElem.GetString());
            var pubKey = NSec.Cryptography.PublicKey.Import(NSec.Cryptography.SignatureAlgorithm.Ed25519, publicKey,
                NSec.Cryptography.KeyBlobFormat.RawPublicKey);
            return NSec.Cryptography.SignatureAlgorithm.Ed25519.Verify(pubKey, Encoding.UTF8.GetBytes(canon),
                signature);
        }
        else if (type == "EcdsaSecp256k1Signature2019")
        {
            // proofValue is a hex string (as produced by Nethereum, MetaMask, Ledger, etc.)
            var signatureHex = valueElem.GetString();

            var signer = new Nethereum.Signer.EthereumMessageSigner();
            // For Ethereum, you always use the original (not hashed) message as string!
            string recoveredAddress = signer.EncodeUTF8AndEcRecover(canon, signatureHex);

            // The expected address, derived from publicKey if needed. If publicKey is the address bytes:
            var expectedAddress = "0x" + BitConverter.ToString(publicKey).Replace("-", "").ToLowerInvariant();
            return string.Equals(
                NormalizeEthAddress(recoveredAddress),
                NormalizeEthAddress(expectedAddress),
                StringComparison.OrdinalIgnoreCase
            );
        }
        else
        {
            throw new NotSupportedException($"Signature type '{type}' is not supported.");
        }
    }

    public async Task<bool> IsRevokedAsync(string statusListCredentialUrl, int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Fetch the status list VC (as per W3C Status List 2021 spec)
            var response = await _httpClient.GetAsync(statusListCredentialUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var encodedList = doc.RootElement
                .GetProperty("credentialSubject")
                .GetProperty("encodedList")
                .GetString();

            byte[] bitstring = Convert.FromBase64String(encodedList);

            int byteIndex = statusListIndex / 8;
            int bitIndex = statusListIndex % 8;
            bool isRevoked = (bitstring[byteIndex] & (1 << (7 - bitIndex))) != 0;

            return isRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve status list.");
            // If status list can't be resolved, consider credential as NOT revoked for safety, or handle per your policy.
            return false;
        }
    }

    private async Task<string> CanonicalizeAsync(string jsonLd, CancellationToken cancellationToken)
    {
        // For demo, not a real URDNA2015: just minified JSON
        var doc = JsonDocument.Parse(jsonLd);
        var canon = JsonSerializer.Serialize(doc.RootElement, new JsonSerializerOptions { WriteIndented = false });
        await Task.Yield();
        return canon;
    }

    private string NormalizeEthAddress(string addr)
    {
        if (string.IsNullOrWhiteSpace(addr)) return "";
        addr = addr.ToLowerInvariant();
        if (!addr.StartsWith("0x")) addr = "0x" + addr;
        return addr;
    }
}