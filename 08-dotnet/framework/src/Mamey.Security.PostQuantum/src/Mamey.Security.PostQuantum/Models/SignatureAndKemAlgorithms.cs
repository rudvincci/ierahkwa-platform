using System;

namespace Mamey.Security.PostQuantum;

/// <summary>
/// Supported signature algorithms for classical, post-quantum, and hybrid operation.
/// Matches the Quantum Resistant MameyNode plan specification.
/// </summary>
public enum SignatureAlgorithm
{
    // Classical (Deprecated - Quantum Vulnerable)
    [Obsolete("Vulnerable to quantum attacks")]
    RSA_SHA256,

    [Obsolete("Vulnerable to quantum attacks")]
    ECDSA_SECP256K1,

    [Obsolete("Vulnerable to quantum attacks")]
    ED25519,

    // Post-Quantum (NIST Approved)
    ML_DSA_44,      // Level 2 security (128-bit)
    ML_DSA_65,      // Level 3 security (192-bit) - RECOMMENDED
    ML_DSA_87,      // Level 5 security (256-bit)

    SLH_DSA_128F,   // Hash-based, Fast variant
    SLH_DSA_128S,   // Hash-based, Small variant
    SLH_DSA_256F,   // High security

    // Hybrid (Transition Mode)
    HYBRID_RSA_MLDSA65,     // RSA-2048 + ML-DSA-65
    HYBRID_ECDSA_MLDSA65    // ECDSA-P256 + ML-DSA-65
}

/// <summary>
/// Supported KEM (key encapsulation) algorithms for classical, post-quantum, and hybrid operation.
/// </summary>
public enum KemAlgorithm
{
    // Classical (Deprecated)
    [Obsolete("Vulnerable to quantum attacks")]
    RSA_OAEP,

    [Obsolete("Vulnerable to quantum attacks")]
    ECDH_P256,

    // Post-Quantum (NIST Approved)
    ML_KEM_512,     // Level 1 security
    ML_KEM_768,     // Level 3 security - RECOMMENDED
    ML_KEM_1024,    // Level 5 security

    // Hybrid
    HYBRID_RSA_MLKEM768
}



