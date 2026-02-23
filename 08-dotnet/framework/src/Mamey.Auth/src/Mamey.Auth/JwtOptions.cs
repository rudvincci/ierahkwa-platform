using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Auth;

/// <summary>
/// JWT-specific configuration options.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Symmetric secret used for classical HMAC-based JWT signatures.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// Token lifetime, in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Logical token issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Intended token audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// When true, JWT tokens will include a post-quantum (ML-DSA) signature
    /// in addition to the classical signature.
    /// </summary>
    public bool UsePostQuantumSigning { get; set; }

    /// <summary>
    /// When true, JWT tokens will include both classical (HMAC) and
    /// post-quantum signatures and validation will require both to be valid.
    /// </summary>
    public bool UseHybridSigning { get; set; }

    /// <summary>
    /// Post-quantum signature algorithm to use when <see cref=\"UsePostQuantumSigning\"/>
    /// or <see cref=\"UseHybridSigning\"/> is enabled. Defaults to ML-DSA-65.
    /// </summary>
    public SignatureAlgorithm SignatureAlgorithm { get; set; } = SignatureAlgorithm.ML_DSA_65;

    /// <summary>
    /// Optional private key material for post-quantum signing. When not provided,
    /// the configured <see cref=\"Mamey.Security.PostQuantum.Interfaces.IPQSigner\"/>
    /// implementation is expected to manage its own key material.
    /// </summary>
    public byte[] PQPrivateKey { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Optional public key material for verifying post-quantum JWT signatures.
    /// When not provided, the configured <see cref=\"Mamey.Security.PostQuantum.Interfaces.IPQSigner\"/>
    /// implementation is expected to expose its public key.
    /// </summary>
    public byte[] PQPublicKey { get; set; } = Array.Empty<byte>();
}