using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;
using Newtonsoft.Json;

namespace Mamey.Security;

public class Signer : ISigner
{
    private readonly IPQSigner? _pqSigner;
    private readonly SecurityOptions? _securityOptions;

    /// <summary>
    /// Default constructor used in older tests and scenarios where PQC is not configured.
    /// Falls back to classical RSA-only signing.
    /// </summary>
    public Signer()
    {
    }

    /// <summary>
    /// Preferred constructor when PQC is configured via DI.
    /// </summary>
    public Signer(SecurityOptions securityOptions, IPQSigner pqSigner)
    {
        _securityOptions = securityOptions ?? throw new ArgumentNullException(nameof(securityOptions));
        _pqSigner = pqSigner ?? throw new ArgumentNullException(nameof(pqSigner));
    }

    private bool PqcEnabled => _pqSigner is not null && _securityOptions is not null;
    private bool UseHybridSignatures => PqcEnabled && _securityOptions!.UseHybridSignatures;

    // Sign using an object, converting it to bytes
    public string Sign(object data, X509Certificate2 certificate)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));

        byte[] dataBytes = ConvertObjectToBytes(data);
        byte[] signedBytes = Sign(dataBytes, certificate);
        return Convert.ToBase64String(signedBytes);
    }

    // Verify using an object, converting it to bytes
    public bool Verify(object data, X509Certificate2 certificate, string signature, bool throwException = false)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));
        if (signature is null) throw new ArgumentNullException(nameof(signature));

        try
        {
            byte[] dataBytes = ConvertObjectToBytes(data);
            byte[] signatureBytes = Convert.FromBase64String(signature);
            return Verify(dataBytes, certificate, signatureBytes, throwException);
        }
        catch (FormatException)
        {
            // Invalid Base64 signature
            if (throwException)
            {
                throw new CryptographicException("Invalid signature format.");
            }

            return false;
        }
        catch (Exception ex)
        {
            if (throwException)
            {
                throw new CryptographicException("Error during verification.", ex);
            }

            return false;
        }
    }

    // Sign using byte array
    public byte[] Sign(byte[] data, X509Certificate2 certificate)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));

        // Always compute classical RSA signature for backward compatibility.
        byte[] classicalSignature = SignWithRsa(data, certificate);

        // If PQC is not enabled or certificate is not PQC, return classical signature only.
        if (!PqcEnabled || !IsPQCCertificate(certificate))
        {
            return classicalSignature;
        }

        // PQC is enabled for this certificate.
        byte[] pqSignature = _pqSigner!.Sign(data);

        if (UseHybridSignatures)
        {
            return CombineSignatures(classicalSignature, pqSignature);
        }

        // PQC-only mode: return ML-DSA signature bytes.
        return pqSignature;
    }

    // Verify using byte array
    public bool Verify(byte[] data, X509Certificate2 certificate, byte[] signature, bool throwException = false)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));
        if (signature is null) throw new ArgumentNullException(nameof(signature));

        try
        {
            bool hasPqcSupport = PqcEnabled && IsPQCCertificate(certificate);

            if (!hasPqcSupport)
            {
                // Classical-only path: try to extract classical part from a hybrid signature if present.
                if (HybridSignature.TryParse(signature, out var hybrid))
                {
                    return VerifyWithRsa(data, certificate, hybrid!.ClassicalSignature, throwException);
                }

                return VerifyWithRsa(data, certificate, signature, throwException);
            }

            // PQC-enabled path.
            if (HybridSignature.TryParse(signature, out var hybridSignature))
            {
                // Hybrid signature: both classical and PQC portions must be valid.
                bool classicalValid = VerifyWithRsa(data, certificate, hybridSignature!.ClassicalSignature, throwException);

                byte[] publicKey = _pqSigner!.PublicKey;
                bool pqValid = _pqSigner.Verify(data, hybridSignature.PostQuantumSignature, publicKey);
                if (!pqValid && throwException)
                {
                    throw new CryptographicException("Post-quantum signature verification failed.");
                }

                return classicalValid && pqValid;
            }

            // PQC-only or classical-only signature when PQC is enabled.
            // Try PQC verification first; fall back to classical verification.
            if (_pqSigner is not null && _pqSigner.PublicKey.Length > 0)
            {
                bool pqValid = _pqSigner.Verify(data, signature, _pqSigner.PublicKey);
                if (pqValid)
                {
                    return true;
                }
            }

            return VerifyWithRsa(data, certificate, signature, throwException);
        }
        catch (CryptographicException)
        {
            if (throwException)
            {
                throw;
            }

            return false;
        }
        catch (Exception ex)
        {
            if (throwException)
            {
                throw new CryptographicException("Error during verification.", ex);
            }

            return false;
        }
    }

    /// <summary>
    /// Determines whether the provided certificate should be treated as a PQC
    /// (ML-DSA) certificate based on its signature algorithm OID.
    /// </summary>
    internal bool IsPQCCertificate(X509Certificate2 certificate)
    {
        if (certificate is null) throw new ArgumentNullException(nameof(certificate));

        var oid = certificate.SignatureAlgorithm?.Value;
        if (string.IsNullOrEmpty(oid))
        {
            return false;
        }

        // NIST FIPS 204 ML-DSA signature algorithm OIDs.
        return oid == "2.16.840.1.101.3.4.3.17" // ML-DSA-44
               || oid == "2.16.840.1.101.3.4.3.18" // ML-DSA-65
               || oid == "2.16.840.1.101.3.4.3.19"; // ML-DSA-87
    }

    /// <summary>
    /// Combines classical and PQ signatures into a single hybrid signature blob.
    /// </summary>
    internal byte[] CombineSignatures(byte[] classicalSignature, byte[] postQuantumSignature)
    {
        if (classicalSignature is null) throw new ArgumentNullException(nameof(classicalSignature));
        if (postQuantumSignature is null) throw new ArgumentNullException(nameof(postQuantumSignature));

        var hybrid = new HybridSignature(classicalSignature, postQuantumSignature);
        return hybrid.CombinedSignature;
    }

    private static byte[] SignWithRsa(byte[] data, X509Certificate2 certificate)
    {
        using RSA? rsa = certificate.GetRSAPrivateKey();
        if (rsa is null)
        {
            throw new CryptographicException("Certificate does not have a private key for signing.");
        }

        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    private static bool VerifyWithRsa(byte[] data, X509Certificate2 certificate, byte[] signature, bool throwException)
    {
        using RSA? rsa = certificate.GetRSAPublicKey();
        if (rsa is null)
        {
            throw new CryptographicException("Certificate does not have a public key for verification.");
        }

        bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        if (!isValid && throwException)
        {
            throw new CryptographicException("Signature verification failed.");
        }

        return isValid;
    }

    // Helper method to convert object to byte array
    private static byte[] ConvertObjectToBytes(object data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        string jsonData = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(jsonData);
    }
}

