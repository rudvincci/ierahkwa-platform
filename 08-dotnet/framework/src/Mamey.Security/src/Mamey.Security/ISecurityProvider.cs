using System.Security.Cryptography.X509Certificates;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Security;

public interface ISecurityProvider
{
    string Encrypt(string data);
    string Decrypt(string data);
    string Hash(string data);
    string GenerateRandomString(int length = 50, bool removeSpecialChars = true);
    string SanitizeUrl(string value);
    string CalculateMd5(string value);
    byte[] Hash(byte[] data);
    string Sign(object data, X509Certificate2 certificate);
    bool Verify(object data, X509Certificate2 certificate, string signature);
    byte[] Sign(byte[] data, X509Certificate2 certificate);
    bool Verify(byte[] data, X509Certificate2 certificate, byte[] signature);

    // Post-quantum operations (ML-DSA / ML-KEM)
    byte[] SignPostQuantum(byte[] data, byte[] privateKey, SignatureAlgorithm algorithm);
    bool VerifyPostQuantum(byte[] data, byte[] signature, byte[] publicKey, SignatureAlgorithm algorithm);
    byte[] EncryptPostQuantum(byte[] data, byte[] recipientPublicKey, KemAlgorithm algorithm);
    byte[] DecryptPostQuantum(byte[] ciphertext, byte[] recipientPrivateKey, KemAlgorithm algorithm);
}
