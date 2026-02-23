using System;
using System.Security.Cryptography;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Mamey.Security.PostQuantum.Algorithms.MLDSA;

namespace Mamey.Security.PostQuantum.Hybrid;

/// <summary>
/// Hybrid signer that combines a classical (RSA/ECDSA) signature with a
/// post-quantum ML-DSA signature into a single <see cref="HybridSignature"/>.
///
/// Classical keys are expected to be in PKCS#8 (private) and SubjectPublicKeyInfo
/// (public) format for RSA or ECDSA. PQ keys use the ML-DSA-65 (Dilithium 3)
/// sizes defined in <see cref="MLDSASigner"/>.
/// </summary>
public sealed class HybridSigner : IHybridCryptoProvider
{
    private readonly SignatureAlgorithm _hybridAlgorithm;

    public HybridSigner(SignatureAlgorithm hybridAlgorithm)
    {
        if (hybridAlgorithm is not SignatureAlgorithm.HYBRID_RSA_MLDSA65
            and not SignatureAlgorithm.HYBRID_ECDSA_MLDSA65)
        {
            throw new ArgumentOutOfRangeException(nameof(hybridAlgorithm), hybridAlgorithm,
                "HybridSigner supports only HYBRID_RSA_MLDSA65 and HYBRID_ECDSA_MLDSA65.");
        }

        _hybridAlgorithm = hybridAlgorithm;
    }

    public HybridSignature SignHybrid(byte[] message, byte[] classicalPrivateKey, byte[] pqPrivateKey)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (classicalPrivateKey is null) throw new ArgumentNullException(nameof(classicalPrivateKey));
        if (pqPrivateKey is null) throw new ArgumentNullException(nameof(pqPrivateKey));

        // Classical signature (RSA or ECDSA with SHA-256)
        byte[] classicalSig = _hybridAlgorithm switch
        {
            SignatureAlgorithm.HYBRID_RSA_MLDSA65 => SignWithRsa(message, classicalPrivateKey),
            SignatureAlgorithm.HYBRID_ECDSA_MLDSA65 => SignWithEcdsa(message, classicalPrivateKey),
            _ => throw new InvalidOperationException("Unsupported hybrid algorithm.")
        };

        // PQ signature using ML-DSA-65
        var pqKeyPair = new PQKeyPair(publicKey: Array.Empty<byte>(), privateKey: pqPrivateKey);
        var pqSigner = new MLDSASigner(SignatureAlgorithm.ML_DSA_65, pqKeyPair);
        byte[] pqSig = pqSigner.Sign(message);

        return new HybridSignature(classicalSig, pqSig);
    }

    public bool VerifyHybrid(byte[] message, HybridSignature signature, byte[] classicalPublicKey, byte[] pqPublicKey)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));
        if (signature is null) throw new ArgumentNullException(nameof(signature));
        if (classicalPublicKey is null) throw new ArgumentNullException(nameof(classicalPublicKey));
        if (pqPublicKey is null) throw new ArgumentNullException(nameof(pqPublicKey));

        bool classicalValid = _hybridAlgorithm switch
        {
            SignatureAlgorithm.HYBRID_RSA_MLDSA65 => VerifyWithRsa(message, signature.ClassicalSignature, classicalPublicKey),
            SignatureAlgorithm.HYBRID_ECDSA_MLDSA65 => VerifyWithEcdsa(message, signature.ClassicalSignature, classicalPublicKey),
            _ => false
        };

        var pqKeyPair = new PQKeyPair(pqPublicKey, privateKey: Array.Empty<byte>());
        var pqSigner = new MLDSASigner(SignatureAlgorithm.ML_DSA_65, pqKeyPair);
        bool pqValid = pqSigner.Verify(message, signature.PostQuantumSignature, pqPublicKey);

        return classicalValid && pqValid;
    }

    public HybridKeyPair GenerateHybridKeyPair(SignatureAlgorithm hybridAlgorithm)
    {
        if (hybridAlgorithm != _hybridAlgorithm)
        {
            throw new ArgumentException("Hybrid algorithm mismatch.", nameof(hybridAlgorithm));
        }

        // Generate classical RSA-2048 or ECDSA P-256 key pair
        byte[] classicalPublic;
        byte[] classicalPrivate;

        if (hybridAlgorithm == SignatureAlgorithm.HYBRID_RSA_MLDSA65)
        {
            using var rsa = RSA.Create(2048);
            classicalPrivate = rsa.ExportPkcs8PrivateKey();
            classicalPublic = rsa.ExportSubjectPublicKeyInfo();
        }
        else
        {
            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            classicalPrivate = ecdsa.ExportPkcs8PrivateKey();
            classicalPublic = ecdsa.ExportSubjectPublicKeyInfo();
        }

        // Generate PQ key pair (ML-DSA-65)
        var pqKeyPair = MLDSASigner.GenerateKeyPair(SignatureAlgorithm.ML_DSA_65);

        return new HybridKeyPair(classicalPublic, classicalPrivate, pqKeyPair.PublicKey, pqKeyPair.PrivateKey);
    }

    private static byte[] SignWithRsa(byte[] message, byte[] privateKeyPkcs8)
    {
        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyPkcs8, out _);
        return rsa.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private static bool VerifyWithRsa(byte[] message, byte[] signature, byte[] publicKeySpki)
    {
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(publicKeySpki, out _);
        return rsa.VerifyData(message, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private static byte[] SignWithEcdsa(byte[] message, byte[] privateKeyPkcs8)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportPkcs8PrivateKey(privateKeyPkcs8, out _);
        return ecdsa.SignData(message, HashAlgorithmName.SHA256);
    }

    private static bool VerifyWithEcdsa(byte[] message, byte[] signature, byte[] publicKeySpki)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportSubjectPublicKeyInfo(publicKeySpki, out _);
        return ecdsa.VerifyData(message, signature, HashAlgorithmName.SHA256);
    }
}


