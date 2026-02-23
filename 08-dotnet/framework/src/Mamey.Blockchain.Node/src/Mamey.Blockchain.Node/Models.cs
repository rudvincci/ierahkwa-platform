using System.Buffers.Binary;
using System.Text;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Blockchain.Node;

/// <summary>
/// Node information
/// </summary>
public class NodeInfo
{
    public string Version { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public ulong BlockCount { get; set; }
    public ulong AccountCount { get; set; }
    public uint PeerCount { get; set; }
    public ulong ConfirmationHeight { get; set; }
}

/// <summary>
/// Block information
/// </summary>
public class Block
{
    public string Hash { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
}

/// <summary>
/// Account information
/// </summary>
public class AccountInfo
{
    public string Account { get; set; } = string.Empty;
    public string HeadBlock { get; set; } = string.Empty;
    public string Representative { get; set; } = string.Empty;
    public string Balance { get; set; } = string.Empty;
    public ulong BlockCount { get; set; }
}

/// <summary>
/// Account history
/// </summary>
public class AccountHistory
{
    public string Account { get; set; } = string.Empty;
    public List<BlockInfo> Blocks { get; set; } = new();
    public int TotalCount { get; set; }
    public bool Success { get; set; }
}

/// <summary>
/// Block information in history
/// </summary>
public class BlockInfo
{
    public string BlockHash { get; set; } = string.Empty;
    public string Previous { get; set; } = string.Empty;
    public string Account { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public byte[] BlockData { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Signature algorithm used for this block (e.g. "ed25519", "ml-dsa-65").
    /// Backed by <see cref="SignatureAlgorithm"/> when possible; stored as string for forward compatibility.
    /// </summary>
    public string SignatureAlgorithm { get; set; } = string.Empty;

    /// <summary>
    /// Primary signature bytes. For classical-only or PQC-only blocks this will
    /// contain the single signature. For hybrid blocks this may contain a
    /// combined representation.
    /// </summary>
    public byte[] Signature { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Optional classical signature component for hybrid blocks.
    /// </summary>
    public byte[]? ClassicalSignature { get; set; }

    /// <summary>
    /// Indicates whether this block is considered quantum-resistant (PQC or hybrid).
    /// </summary>
    public bool QuantumResistant { get; set; }
}

/// <summary>
/// Represents a Mamey transaction with support for classical, PQC, and hybrid signatures.
/// </summary>
public sealed class MameyTransaction
{
    /// <summary>
    /// Serialization format version.
    /// 1 = classical-only, 2 = PQC / hybrid extended format.
    /// </summary>
    public byte Version { get; set; } = 2;

    /// <summary>
    /// Application-specific transaction payload (fixed-size in sizing examples).
    /// </summary>
    public byte[] Data { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Signature algorithm used for this transaction (e.g. ML-DSA-65).
    /// </summary>
    public SignatureAlgorithm SignatureAlgorithm { get; set; } = SignatureAlgorithm.ML_DSA_65;

    /// <summary>
    /// Primary signature bytes (classical, PQC, or combined, depending on mode).
    /// </summary>
    public byte[] Signature { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Indicates whether this transaction uses a hybrid signature.
    /// </summary>
    public bool IsHybrid { get; set; }

    /// <summary>
    /// Optional classical signature component for hybrid mode.
    /// </summary>
    public byte[]? ClassicalSignature { get; set; }

    /// <summary>
    /// Optional PQC signature component for hybrid mode.
    /// </summary>
    public byte[]? PQSignature { get; set; }

    /// <summary>
    /// Total size of the effective signature payload (in bytes).
    /// </summary>
    public int SignatureSize => Signature?.Length ?? 0;

    /// <summary>
    /// Indicates whether this transaction is considered quantum-resistant
    /// (PQC-only or hybrid).
    /// </summary>
    public bool IsQuantumResistant { get; set; }

    /// <summary>
    /// Serialize this transaction into a compact binary representation:
    /// [version][data_length][data][sig_algorithm_length][sig_algorithm][signature_length][signature]
    /// Version 1 omits PQC/hybrid metadata; version 2 uses the extended fields.
    /// </summary>
    public byte[] Serialize()
    {
        var data = Data ?? Array.Empty<byte>();
        var algoName = SignatureAlgorithm.ToString();
        var algoBytes = Encoding.ASCII.GetBytes(algoName);
        var signature = Signature ?? Array.Empty<byte>();

        // Layout: version(1) | dataLen(4) | data | algoLen(2) | algoBytes | sigLen(4) | signature
        var totalLength = 1 + 4 + data.Length + 2 + algoBytes.Length + 4 + signature.Length;
        var buffer = new byte[totalLength];
        var offset = 0;

        buffer[offset++] = Version;

        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(offset, 4), data.Length);
        offset += 4;
        data.CopyTo(buffer.AsSpan(offset));
        offset += data.Length;

        BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(offset, 2), (ushort)algoBytes.Length);
        offset += 2;
        algoBytes.CopyTo(buffer.AsSpan(offset));
        offset += algoBytes.Length;

        BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan(offset, 4), signature.Length);
        offset += 4;
        signature.CopyTo(buffer.AsSpan(offset));

        return buffer;
    }

    /// <summary>
    /// Deserialize a transaction from its binary representation. Supports
    /// version 1 (classical-only) and version 2 (PQC/hybrid-ready).
    /// </summary>
    public static MameyTransaction Deserialize(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
        {
            throw new ArgumentException("Buffer cannot be empty.", nameof(buffer));
        }

        var offset = 0;
        var version = buffer[offset++];

        if (buffer.Length < offset + 4)
        {
            throw new ArgumentException("Buffer too small to contain data length.", nameof(buffer));
        }

        var dataLength = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset, 4));
        offset += 4;

        if (dataLength < 0 || buffer.Length < offset + dataLength)
        {
            throw new ArgumentException("Invalid data length in transaction buffer.", nameof(buffer));
        }

        var data = buffer.Slice(offset, dataLength).ToArray();
        offset += dataLength;

        if (buffer.Length < offset + 2)
        {
            throw new ArgumentException("Buffer too small to contain algorithm length.", nameof(buffer));
        }

        var algoLength = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(offset, 2));
        offset += 2;

        if (buffer.Length < offset + algoLength)
        {
            throw new ArgumentException("Invalid algorithm length in transaction buffer.", nameof(buffer));
        }

        var algoBytes = buffer.Slice(offset, algoLength).ToArray();
        offset += algoLength;
        var algoName = Encoding.ASCII.GetString(algoBytes);

        if (buffer.Length < offset + 4)
        {
            throw new ArgumentException("Buffer too small to contain signature length.", nameof(buffer));
        }

        var sigLength = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(offset, 4));
        offset += 4;

        if (sigLength < 0 || buffer.Length < offset + sigLength)
        {
            throw new ArgumentException("Invalid signature length in transaction buffer.", nameof(buffer));
        }

        var signature = buffer.Slice(offset, sigLength).ToArray();

        var tx = new MameyTransaction
        {
            Version = version,
            Data = data,
            Signature = signature,
            SignatureAlgorithm = Enum.TryParse<SignatureAlgorithm>(algoName, ignoreCase: true, out var parsed)
                ? parsed
                : SignatureAlgorithm.ML_DSA_65
        };

        // Version-based compatibility: V1 = classical-only, V2 = PQC/hybrid-aware.
        if (version == 1)
        {
            tx.IsHybrid = false;
            tx.IsQuantumResistant = false;
            tx.ClassicalSignature = signature;
            tx.PQSignature = null;
        }
        else
        {
            // For now we treat any ML-DSA / HYBRID algorithm as quantum-resistant.
            tx.IsQuantumResistant = algoName.ToLowerInvariant().Contains("ml_dsa")
                                    || algoName.ToLowerInvariant().Contains("hybrid");
        }

        return tx;
    }
}

/// <summary>
/// Publish block result
/// </summary>
public class PublishBlockResult
{
    public string BlockHash { get; set; } = string.Empty;
    public bool Accepted { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Peer information
/// </summary>
public class PeerInfo
{
    public string PeerId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public DateTime LastSeen { get; set; }
}


























