using System.Text.Json;
using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Crypto;
using Mamey.Auth.Decentralized.Utilities;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Handler for Verifiable Credentials in JSON-LD format
/// </summary>
public class VcJsonLdHandler
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly ILogger<VcJsonLdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the VcJsonLdHandler class
    /// </summary>
    /// <param name="keyGenerator">The key generator service</param>
    /// <param name="logger">The logger</param>
    public VcJsonLdHandler(IKeyGenerator keyGenerator, ILogger<VcJsonLdHandler> logger)
    {
        _keyGenerator = keyGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Signs a Verifiable Credential with a proof
    /// </summary>
    /// <param name="credential">The verifiable credential</param>
    /// <param name="privateKey">The private key for signing</param>
    /// <param name="verificationMethod">The verification method</param>
    /// <param name="algorithm">The signing algorithm</param>
    /// <returns>The signed verifiable credential</returns>
    public VerifiableCredential SignCredential(VerifiableCredential credential, byte[] privateKey, string verificationMethod, string algorithm = "Ed25519")
    {
        if (credential == null)
            throw new ArgumentNullException(nameof(credential));

        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));

        if (string.IsNullOrEmpty(verificationMethod))
            throw new ArgumentException("Verification method cannot be null or empty", nameof(verificationMethod));

        if (!credential.IsValid())
            throw new VcValidationException("Invalid verifiable credential");

        try
        {
            var provider = _keyGenerator.GetProvider(algorithm);
            
            // Create a copy of the credential
            var signedCredential = credential.Clone();

            // Create the proof
            var proof = new Proof
            {
                Type = GetProofType(algorithm),
                VerificationMethod = verificationMethod,
                Created = DateTime.UtcNow,
                ProofPurpose = "assertionMethod"
            };

            // Create the canonical form for signing
            var canonicalForm = CreateCanonicalForm(signedCredential, proof);
            var signature = provider.Sign(Encoding.UTF8.GetBytes(canonicalForm), privateKey);

            // Set the signature value
            proof.SetSignatureValue(Convert.ToBase64String(signature));

            // Add the proof to the credential
            signedCredential.Proof = proof;

            return signedCredential;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign verifiable credential with JSON-LD proof");
            throw new VcException("Failed to sign verifiable credential with JSON-LD proof", ex);
        }
    }

    /// <summary>
    /// Verifies a Verifiable Credential with a JSON-LD proof
    /// </summary>
    /// <param name="credential">The verifiable credential</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>True if the credential is valid, false otherwise</returns>
    public bool VerifyCredential(VerifiableCredential credential, byte[] publicKey, string algorithm = "Ed25519")
    {
        if (credential == null)
            throw new ArgumentNullException(nameof(credential));

        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        if (!credential.IsValid())
            return false;

        if (credential.Proof == null)
            return false;

        try
        {
            var provider = _keyGenerator.GetProvider(algorithm);
            
            // Extract the signature from the proof
            var signatureValue = credential.Proof.GetSignatureValue();
            if (string.IsNullOrEmpty(signatureValue))
                return false;

            var signature = Convert.FromBase64String(signatureValue);

            // Create the canonical form for verification
            var canonicalForm = CreateCanonicalForm(credential, credential.Proof);

            // Verify the signature
            return provider.Verify(Encoding.UTF8.GetBytes(canonicalForm), signature, publicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify verifiable credential with JSON-LD proof");
            return false;
        }
    }

    /// <summary>
    /// Signs a Verifiable Presentation with a proof
    /// </summary>
    /// <param name="presentation">The verifiable presentation</param>
    /// <param name="privateKey">The private key for signing</param>
    /// <param name="verificationMethod">The verification method</param>
    /// <param name="algorithm">The signing algorithm</param>
    /// <returns>The signed verifiable presentation</returns>
    public VerifiablePresentation SignPresentation(VerifiablePresentation presentation, byte[] privateKey, string verificationMethod, string algorithm = "Ed25519")
    {
        if (presentation == null)
            throw new ArgumentNullException(nameof(presentation));

        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));

        if (string.IsNullOrEmpty(verificationMethod))
            throw new ArgumentException("Verification method cannot be null or empty", nameof(verificationMethod));

        if (!presentation.IsValid())
            throw new VcValidationException("Invalid verifiable presentation");

        try
        {
            var provider = _keyGenerator.GetProvider(algorithm);
            
            // Create a copy of the presentation
            var signedPresentation = presentation.Clone();

            // Create the proof
            var proof = new Proof
            {
                Type = GetProofType(algorithm),
                VerificationMethod = verificationMethod,
                Created = DateTime.UtcNow,
                ProofPurpose = "authentication"
            };

            // Create the canonical form for signing
            var canonicalForm = CreateCanonicalForm(signedPresentation, proof);
            var signature = provider.Sign(Encoding.UTF8.GetBytes(canonicalForm), privateKey);

            // Set the signature value
            proof.SetSignatureValue(Convert.ToBase64String(signature));

            // Add the proof to the presentation
            signedPresentation.Proof = proof;

            return signedPresentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign verifiable presentation with JSON-LD proof");
            throw new VcException("Failed to sign verifiable presentation with JSON-LD proof", ex);
        }
    }

    /// <summary>
    /// Verifies a Verifiable Presentation with a JSON-LD proof
    /// </summary>
    /// <param name="presentation">The verifiable presentation</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>True if the presentation is valid, false otherwise</returns>
    public bool VerifyPresentation(VerifiablePresentation presentation, byte[] publicKey, string algorithm = "Ed25519")
    {
        if (presentation == null)
            throw new ArgumentNullException(nameof(presentation));

        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        if (!presentation.IsValid())
            return false;

        if (presentation.Proof == null)
            return false;

        try
        {
            var provider = _keyGenerator.GetProvider(algorithm);
            
            // Extract the signature from the proof
            var signatureValue = presentation.Proof.GetSignatureValue();
            if (string.IsNullOrEmpty(signatureValue))
                return false;

            var signature = Convert.FromBase64String(signatureValue);

            // Create the canonical form for verification
            var canonicalForm = CreateCanonicalForm(presentation, presentation.Proof);

            // Verify the signature
            return provider.Verify(Encoding.UTF8.GetBytes(canonicalForm), signature, publicKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify verifiable presentation with JSON-LD proof");
            return false;
        }
    }

    private string CreateCanonicalForm(object document, Proof proof)
    {
        // Create a copy of the document without the proof
        var documentJson = JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        var documentObj = JsonSerializer.Deserialize<JsonElement>(documentJson);
        var documentDict = JsonSerializer.Deserialize<Dictionary<string, object>>(documentJson) ?? new Dictionary<string, object>();

        // Remove the proof from the document
        documentDict.Remove("proof");

        // Create the canonical form
        var canonicalForm = JsonSerializer.Serialize(documentDict, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        return canonicalForm;
    }

    private string GetProofType(string algorithm)
    {
        return algorithm switch
        {
            "Ed25519" => "Ed25519Signature2020",
            "Secp256k1" => "EcdsaSecp256k1Signature2019",
            "RSA" => "RsaSignature2018",
            _ => "JsonWebSignature2020"
        };
    }
}
