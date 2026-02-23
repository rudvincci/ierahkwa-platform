namespace Mamey.Security;

public interface IPrivateKey
{
    string EncryptedPrivateKey { get; }
    string? PrivateKeySignature { get; }
    void BindKey(string privateKeySignature) { }
    void BindSignature(string privateKeySignature) { }
}