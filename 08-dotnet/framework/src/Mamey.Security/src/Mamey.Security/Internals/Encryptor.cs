using System.Security.Cryptography;
using System.Text;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Algorithms.MLKEM;
using Mamey.Security.PostQuantum.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.Security.Internals;

internal sealed class Encryptor : IEncryptor
{
    private readonly ILogger<Encryptor> _logger;
    private const int AesKeySize = 32; // 256 bits
    private const int AesIvSize = 32; // 128 bits
    private const int TripleDesKeySize = 24; // 192 bits
    private const int TripleDesIvSize = 8; // 64 bits

    private readonly MLKEMEncryptor _mlkemEncryptor = new();

    public Encryptor(ILogger<Encryptor> logger)
    {
        _logger = logger;
    }

    public string Encrypt(string data, string key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES)
    {
        ValidateInput(data, key);
        return algorithm switch 
        {
            EncryptionAlgorithms.AES => EncryptAes(data, key),
            EncryptionAlgorithms.TripleDes => EncryptTripleDes(data, key),
            EncryptionAlgorithms.RSA => EncryptRsa(data, key),
            EncryptionAlgorithms.MLKEM512 or EncryptionAlgorithms.MLKEM768 or EncryptionAlgorithms.MLKEM1024
                => Convert.ToBase64String(EncryptMLKEM(Encoding.UTF8.GetBytes(data), Convert.FromBase64String(key), algorithm)),
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not supported.")
        };
    }

    public string Decrypt(string data, string key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES)
    {
        ValidateInput(data, key);

        return algorithm switch
        {
            EncryptionAlgorithms.AES => DecryptAes(data, key),
            EncryptionAlgorithms.TripleDes => DecryptTripleDes(data, key),
            EncryptionAlgorithms.RSA => DecryptRsa(data, key),
            EncryptionAlgorithms.MLKEM512 or EncryptionAlgorithms.MLKEM768 or EncryptionAlgorithms.MLKEM1024
                => Encoding.UTF8.GetString(DecryptMLKEM(Convert.FromBase64String(data), Convert.FromBase64String(key), algorithm)),
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not supported.")
        };
    }

    public byte[] Encrypt(byte[] data, byte[] iv, byte[] key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES)
    {
        ValidateInput(data);
        return algorithm switch
        {
            EncryptionAlgorithms.AES => EncryptAes(data, iv, key),
            EncryptionAlgorithms.TripleDes => EncryptTripleDes(data, iv, key),
            EncryptionAlgorithms.RSA => EncryptRsa(data, iv, key),
            EncryptionAlgorithms.MLKEM512 or EncryptionAlgorithms.MLKEM768 or EncryptionAlgorithms.MLKEM1024
                => EncryptMLKEM(data, key, algorithm),
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not supported.")
        };
    }

    public byte[] Decrypt(byte[] data, byte[] iv, byte[] key, EncryptionAlgorithms algorithm = EncryptionAlgorithms.AES)
    {
        ValidateInput(data);
        return algorithm switch
        {
            EncryptionAlgorithms.AES => DecryptAes(data, iv, key),
            EncryptionAlgorithms.TripleDes => DecryptTripleDes(data, iv, key),
            EncryptionAlgorithms.RSA => DecryptRsa(data, iv, key),
            EncryptionAlgorithms.MLKEM512 or EncryptionAlgorithms.MLKEM768 or EncryptionAlgorithms.MLKEM1024
                => DecryptMLKEM(data, key, algorithm),
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not supported.")
        };
    }


    #region AES
    private string EncryptAes(string data, string key)
    {
        ValidateKeySize(key, AesIvSize);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.GenerateIV();
        var iv = Convert.ToBase64String(aes.IV);
        var transform = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(data);
        }

        return iv + Convert.ToBase64String(memoryStream.ToArray());
    }
    private byte[] EncryptAes(byte[] data, byte[] iv, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        csEncrypt.Write(data, 0, data.Length);
        csEncrypt.FlushFinalBlock();

        return msEncrypt.ToArray();
    }
    private string DecryptAes(string data, string key)
    {
        ValidateKeySize(key, AesKeySize);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Convert.FromBase64String(data.Substring(0, 24));
        var transform = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(Convert.FromBase64String(data.Substring(24)));
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
    private byte[] DecryptAes(byte[] data, byte[] iv, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        var transform = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(data);
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
        using var outputStream = new MemoryStream();
        cryptoStream.CopyTo(outputStream);

        return outputStream.ToArray();
    }
    #endregion

    #region TripleDes
    private string EncryptTripleDes(string data, string key)
    {
        ValidateKeySize(key, TripleDesKeySize);

        using var tripleDes = TripleDES.Create();
        tripleDes.Key = Encoding.UTF8.GetBytes(key);
        tripleDes.GenerateIV();
        var iv = Convert.ToBase64String(tripleDes.IV);
        var transform = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(data);
        }

        return iv + Convert.ToBase64String(memoryStream.ToArray());
    }
    private string DecryptTripleDes(string data, string key)
    {
        ValidateKeySize(key, TripleDesKeySize);

        using var tripleDes = TripleDES.Create();
        tripleDes.Key = Encoding.UTF8.GetBytes(key);
        tripleDes.IV = Convert.FromBase64String(data.Substring(0, 12));
        var transform = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);
        using var memoryStream = new MemoryStream(Convert.FromBase64String(data.Substring(12)));
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        return streamReader.ReadToEnd();
    }
    private byte[] EncryptTripleDes(byte[] data, byte[] iv, byte[] key)
    {
        using var tripleDes = TripleDES.Create();
        tripleDes.Key = key;
        tripleDes.IV = iv;
        var transform = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);
        using var memoryStream = new MemoryStream();
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return memoryStream.ToArray();
    }
    private byte[] DecryptTripleDes(byte[] data, byte[] iv, byte[] key)
    {
        using var tripleDes = TripleDES.Create();
        tripleDes.Key = key;
        tripleDes.IV = iv;
        var transform = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);
        using var memoryStream = new MemoryStream(data);
        using var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read);
        using var outputStream = new MemoryStream();
        cryptoStream.CopyTo(outputStream);

        return outputStream.ToArray();
    }
    #endregion

    #region Rsa
    private string EncryptRsa(string data, string key)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(key);
        var encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(data), true);

        return Convert.ToBase64String(encryptedData);
    }
    private string DecryptRsa(string data, string key)
    {
        using var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(key);
        var decryptedData = rsa.Decrypt(Convert.FromBase64String(data), true);

        return Encoding.UTF8.GetString(decryptedData);
    }    
    private byte[] EncryptRsa(byte[] data, byte[] iv, byte[] key)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPublicKey(key, out _);
        var encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

        return encryptedData;
    }
    private byte[] DecryptRsa(byte[] data, byte[] iv, byte[] key)
    {
        using var rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(key, out _);
        var decryptedData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);

        return decryptedData;
    }
    #endregion


    #region ML-KEM helpers

    public byte[] EncryptMLKEM(byte[] data, byte[] recipientPublicKey, EncryptionAlgorithms algorithm)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (recipientPublicKey is null) throw new ArgumentNullException(nameof(recipientPublicKey));

        var kemAlg = algorithm switch
        {
            EncryptionAlgorithms.MLKEM512 => KemAlgorithm.ML_KEM_512,
            EncryptionAlgorithms.MLKEM768 => KemAlgorithm.ML_KEM_768,
            EncryptionAlgorithms.MLKEM1024 => KemAlgorithm.ML_KEM_1024,
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not an ML-KEM variant.")
        };

        var (kemCiphertext, ciphertext, nonce, tag) =
            _mlkemEncryptor.EncryptMLKEM(kemAlg, recipientPublicKey, data, null);

        return CombineBytes(kemCiphertext.Value, nonce, tag, ciphertext);
    }

    public byte[] DecryptMLKEM(byte[] data, byte[] recipientPrivateKey, EncryptionAlgorithms algorithm)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (recipientPrivateKey is null) throw new ArgumentNullException(nameof(recipientPrivateKey));

        var kemAlg = algorithm switch
        {
            EncryptionAlgorithms.MLKEM512 => KemAlgorithm.ML_KEM_512,
            EncryptionAlgorithms.MLKEM768 => KemAlgorithm.ML_KEM_768,
            EncryptionAlgorithms.MLKEM1024 => KemAlgorithm.ML_KEM_1024,
            _ => throw new NotSupportedException($"The encryption algorithm {algorithm} is not an ML-KEM variant.")
        };

        SplitBytes(data, out var kemBytes, out var nonce, out var tag, out var ciphertext);
        var kemCiphertext = new PQCiphertext(kemBytes);
        return _mlkemEncryptor.DecryptMLKEM(kemAlg, recipientPrivateKey, kemCiphertext, ciphertext, nonce, tag);
    }

    private static byte[] CombineBytes(byte[] kemCiphertext, byte[] nonce, byte[] tag, byte[] ciphertext)
    {
        if (kemCiphertext is null) throw new ArgumentNullException(nameof(kemCiphertext));
        if (nonce is null) throw new ArgumentNullException(nameof(nonce));
        if (tag is null) throw new ArgumentNullException(nameof(tag));
        if (ciphertext is null) throw new ArgumentNullException(nameof(ciphertext));

        // Format: [kemLen:4][nonceLen:4][tagLen:4][kemCt][nonce][tag][ciphertext]
        var kemLen = (uint)kemCiphertext.Length;
        var nonceLen = (uint)nonce.Length;
        var tagLen = (uint)tag.Length;

        var totalLen = 4 + 4 + 4 + kemCiphertext.Length + nonce.Length + tag.Length + ciphertext.Length;
        var buffer = new byte[totalLen];
        var offset = 0;

        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), kemLen);
        offset += 4;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), nonceLen);
        offset += 4;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), tagLen);
        offset += 4;

        Buffer.BlockCopy(kemCiphertext, 0, buffer, offset, kemCiphertext.Length);
        offset += kemCiphertext.Length;
        Buffer.BlockCopy(nonce, 0, buffer, offset, nonce.Length);
        offset += nonce.Length;
        Buffer.BlockCopy(tag, 0, buffer, offset, tag.Length);
        offset += tag.Length;
        Buffer.BlockCopy(ciphertext, 0, buffer, offset, ciphertext.Length);

        return buffer;
    }

    private static void SplitBytes(byte[] combined, out byte[] kemCiphertext, out byte[] nonce, out byte[] tag, out byte[] ciphertext)
    {
        if (combined is null) throw new ArgumentNullException(nameof(combined));
        if (combined.Length < 12)
        {
            throw new CryptographicException("Invalid ML-KEM ciphertext format.");
        }

        var offset = 0;
        var kemLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(combined.AsSpan(offset, 4));
        offset += 4;
        var nonceLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(combined.AsSpan(offset, 4));
        offset += 4;
        var tagLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(combined.AsSpan(offset, 4));
        offset += 4;

        var expectedHeader = 12 + (int)kemLen + (int)nonceLen + (int)tagLen;
        if (combined.Length < expectedHeader)
        {
            throw new CryptographicException("Invalid ML-KEM ciphertext length.");
        }

        kemCiphertext = new byte[kemLen];
        Buffer.BlockCopy(combined, offset, kemCiphertext, 0, (int)kemLen);
        offset += (int)kemLen;

        nonce = new byte[nonceLen];
        Buffer.BlockCopy(combined, offset, nonce, 0, (int)nonceLen);
        offset += (int)nonceLen;

        tag = new byte[tagLen];
        Buffer.BlockCopy(combined, offset, tag, 0, (int)tagLen);
        offset += (int)tagLen;

        var remaining = combined.Length - offset;
        if (remaining < 0)
        {
            throw new CryptographicException("Invalid ML-KEM ciphertext payload.");
        }

        ciphertext = new byte[remaining];
        Buffer.BlockCopy(combined, offset, ciphertext, 0, remaining);
    }

    #endregion

    #region Helper Methods
    private void ValidateInput(string data, string key)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Encryption key cannot be empty.", nameof(key));

    }
    private void ValidateInput(byte[] data)
    {
        if (data is null || !data.Any())
            throw new ArgumentException("Data to be encrypted/decrypted cannot be empty.", nameof(data));
    }

    private void ValidateKeySize(string key, int requiredKeySize)
    {
        if (key.Length != requiredKeySize)
            throw new ArgumentException($"Encryption key must be {requiredKeySize} bytes long.", nameof(key));
    }
    #endregion
}
