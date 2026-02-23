namespace Mamey.Security.PostQuantum.Models;

/// <summary>
/// Generic container for a public/private key pair for PQC or hybrid algorithms.
/// </summary>
public sealed class PQKeyPair
{
    public byte[] PublicKey { get; }
    public byte[] PrivateKey { get; }

    public PQKeyPair(byte[] publicKey, byte[] privateKey)
    {
        PublicKey = publicKey;
        PrivateKey = privateKey;
    }
}

/// <summary>
/// Represents a post-quantum or hybrid signature blob.
/// </summary>
public sealed class PQSignature
{
    public byte[] Value { get; }

    public PQSignature(byte[] value) => Value = value;
}

/// <summary>
/// Represents a post-quantum ciphertext (for KEM/ML-KEM).
/// </summary>
public sealed class PQCiphertext
{
    public byte[] Value { get; }

    public PQCiphertext(byte[] value) => Value = value;
}

/// <summary>
/// Hybrid signature combining classical and PQ signatures and providing a
/// deterministic binary representation.
///
/// Serialized format (versioned, little-endian lengths):
/// [version:1][classicalLen:4][pqLen:4][classicalBytes][pqBytes]
/// </summary>
public sealed class HybridSignature
{
    public const byte CurrentVersion = 1;

    public byte[] ClassicalSignature { get; }
    public byte[] PostQuantumSignature { get; }

    /// <summary>
    /// Full serialized representation of the hybrid signature.
    /// </summary>
    public byte[] CombinedSignature { get; }

    public HybridSignature(byte[] classicalSignature, byte[] postQuantumSignature)
    {
        ClassicalSignature = classicalSignature ?? throw new System.ArgumentNullException(nameof(classicalSignature));
        PostQuantumSignature = postQuantumSignature ?? throw new System.ArgumentNullException(nameof(postQuantumSignature));
        CombinedSignature = Serialize(classicalSignature, postQuantumSignature);
    }

    private static byte[] Serialize(byte[] classical, byte[] pq)
    {
        var classicalLen = (uint)classical.Length;
        var pqLen = (uint)pq.Length;

        var buffer = new byte[1 + 4 + 4 + classical.Length + pq.Length];
        int offset = 0;

        buffer[offset++] = CurrentVersion;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), classicalLen);
        offset += 4;
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), pqLen);
        offset += 4;

        System.Buffer.BlockCopy(classical, 0, buffer, offset, classical.Length);
        offset += classical.Length;
        System.Buffer.BlockCopy(pq, 0, buffer, offset, pq.Length);

        return buffer;
    }

    /// <summary>
    /// Parse a serialized hybrid signature produced by <see cref="HybridSignature"/>.
    /// Returns <c>true</c> on success and populates <paramref name="signature"/>.
    /// </summary>
    public static bool TryParse(byte[] combined, out HybridSignature? signature)
    {
        signature = null;
        if (combined is null || combined.Length < 1 + 4 + 4)
        {
            return false;
        }

        int offset = 0;
        byte version = combined[offset++];
        if (version != CurrentVersion)
        {
            return false; // unsupported version
        }

        var span = combined.AsSpan(offset);
        if (span.Length < 8)
        {
            return false;
        }

        uint classicalLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(0, 4));
        uint pqLen = System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4, 4));

        int totalLen = 1 + 4 + 4 + checked((int)classicalLen) + checked((int)pqLen);
        if (combined.Length != totalLen)
        {
            return false;
        }

        var classical = new byte[classicalLen];
        var pq = new byte[pqLen];
        System.Buffer.BlockCopy(combined, 1 + 4 + 4, classical, 0, (int)classicalLen);
        System.Buffer.BlockCopy(combined, 1 + 4 + 4 + (int)classicalLen, pq, 0, (int)pqLen);

        signature = new HybridSignature(classical, pq);
        return true;
    }
}

/// <summary>
/// Hybrid key pair for transition mode (classical + PQ keys).
/// </summary>
public sealed class HybridKeyPair
{
    public byte[] ClassicalPublicKey { get; }
    public byte[] ClassicalPrivateKey { get; }
    public byte[] PostQuantumPublicKey { get; }
    public byte[] PostQuantumPrivateKey { get; }

    public HybridKeyPair(
        byte[] classicalPublicKey,
        byte[] classicalPrivateKey,
        byte[] postQuantumPublicKey,
        byte[] postQuantumPrivateKey)
    {
        ClassicalPublicKey = classicalPublicKey;
        ClassicalPrivateKey = classicalPrivateKey;
        PostQuantumPublicKey = postQuantumPublicKey;
        PostQuantumPrivateKey = postQuantumPrivateKey;
    }
}



