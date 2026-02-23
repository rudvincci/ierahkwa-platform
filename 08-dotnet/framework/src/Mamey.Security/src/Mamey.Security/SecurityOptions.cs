using System;
using System.Collections.Generic;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security;

public class SecurityOptions : Mamey.WebApi.Security.SecurityOptions
{
    public string EncryptionKey { get; set; }
    public RecoveryOptions Recovery { get; set; }
    public EncryptionOptions Encryption { get; set; }
    public CertificateOptions Certificate { get; set; }
    
    /// <summary>
    /// Enables post-quantum cryptography features in <see cref="SecurityProvider"/>.
    /// Defaults to <c>false</c> to preserve classical-only behaviour.
    /// </summary>
    public bool EnablePostQuantum { get; set; } = false;

    /// <summary>
    /// When enabled, the signer will emit hybrid signatures that combine
    /// a classical RSA/ECDSA signature with a post-quantum ML-DSA signature.
    /// </summary>
    public bool UseHybridSignatures { get; set; } = false;

    /// <summary>
    /// Default post-quantum signature algorithm used when none is explicitly specified.
    /// </summary>
    public SignatureAlgorithm DefaultSignatureAlgorithm { get; set; } = SignatureAlgorithm.ML_DSA_65;

    /// <summary>
    /// Default post-quantum KEM algorithm used for ML-KEM key encapsulation.
    /// </summary>
    public KemAlgorithm DefaultKemAlgorithm { get; set; } = KemAlgorithm.ML_KEM_768;

    /// <summary>
    /// Optional deadline by which all classical-only keys should be migrated
    /// to post-quantum or hybrid equivalents.
    /// </summary>
    public DateTimeOffset? QuantumMigrationDeadline { get; set; }
    public class RecoveryOptions
    {
        public string Issuer { get; set; }
        public TimeSpan? Expiry { get; set; }
        public int ExpiryMinutes { get; set; }
        public IEnumerable<string> AllowedEndpoints { get; set; }
    }
    public class EncryptionOptions
    {
        public bool Enabled { get; set; }
        public string Key { get; set; }
    }
    
}
