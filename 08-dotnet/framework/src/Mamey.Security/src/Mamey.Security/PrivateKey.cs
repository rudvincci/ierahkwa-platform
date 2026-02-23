using System.Security.Cryptography.X509Certificates;

namespace Mamey.Security;

[Serializable]
public class PrivateKey : IPrivateKey
{
    public PrivateKey()
    { }

    public PrivateKey(string encryptedPrivateKey, string? signature)
    {
        if (string.IsNullOrEmpty(encryptedPrivateKey))
        {
            throw new ArgumentException($"'{nameof(encryptedPrivateKey)}' cannot be null or empty.", nameof(encryptedPrivateKey));
        }

        EncryptedPrivateKey = encryptedPrivateKey;
        PrivateKeySignature = signature ?? string.Empty;
    }

    /// <summary>
    /// Encrypted private key value.
    /// </summary>
    public string EncryptedPrivateKey { get; internal set; } = string.Empty;
    /// <summary>
    /// Signature of encrypted private key.
    /// </summary>
    public string? PrivateKeySignature { get; internal set; } = string.Empty;


    protected void BindKey(string encryptedPrivateKeyHash) { EncryptedPrivateKey = encryptedPrivateKeyHash; }
    protected void BindSignature(string privateKeySignature) { PrivateKeySignature = privateKeySignature; }
}
public interface ICertificateResult<TPrivateKey> where TPrivateKey : class, IPrivateKey
{
    TPrivateKey PrivateKey { get; init; }
    X509Certificate2 Certificate { get; init; }
    string? Key { get; init; }
}
public record CertificateResult<T>(T PrivateKey, X509Certificate2 Certificate, string? Key = null) : ICertificateResult<T> where T : class, IPrivateKey;


public record class PrivateKeyResult<T>(T PrivateKey, string? Key = null) : IPrivateKeyResult<T> where T : IPrivateKey;

