namespace Mamey.Security;

public interface IEncryptor
{
    string Encrypt(string data, string key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES);
    string Decrypt(string data, string key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES);
    byte[] Encrypt(byte[] data, byte[] iv, byte[] key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES);
    byte[] Decrypt(byte[] data, byte[] iv, byte[] key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES);

    // Post-quantum helpers (ML-KEM + AES-256-GCM)
    byte[] EncryptMLKEM(byte[] data, byte[] recipientPublicKey, EncryptionAlgorithms algorithm);
    byte[] DecryptMLKEM(byte[] data, byte[] recipientPrivateKey, EncryptionAlgorithms algorithm);
}
