using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.Security.Tests.Shared.Utilities;

/// <summary>
/// Provides test X.509 certificates for testing.
/// </summary>
public static class TestCertificates
{
    /// <summary>
    /// Creates a self-signed X.509 certificate with private key for testing.
    /// </summary>
    public static X509Certificate2 CreateSelfSignedCertificate(string subjectName = "CN=Test Certificate")
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(1)
        );

        return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "password"), "password", X509KeyStorageFlags.Exportable);
    }

    /// <summary>
    /// Creates a self-signed X.509 certificate without private key for testing.
    /// </summary>
    public static X509Certificate2 CreatePublicCertificate(string subjectName = "CN=Test Certificate")
    {
        var certWithPrivateKey = CreateSelfSignedCertificate(subjectName);
        var publicKeyBytes = certWithPrivateKey.Export(X509ContentType.Cert);
        return new X509Certificate2(publicKeyBytes);
    }

    /// <summary>
    /// Creates a certificate with custom key size.
    /// </summary>
    public static X509Certificate2 CreateCertificateWithKeySize(int keySize, string subjectName = "CN=Test Certificate")
    {
        using var rsa = RSA.Create(keySize);
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        var certificate = request.CreateSelfSigned(
            DateTimeOffset.Now.AddDays(-1),
            DateTimeOffset.Now.AddYears(1)
        );

        return new X509Certificate2(certificate.Export(X509ContentType.Pfx, "password"), "password", X509KeyStorageFlags.Exportable);
    }
}



