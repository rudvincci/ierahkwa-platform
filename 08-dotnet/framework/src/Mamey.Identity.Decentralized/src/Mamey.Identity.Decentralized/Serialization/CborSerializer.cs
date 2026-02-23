using System.Buffers;
using System.Formats.Cbor;
using System.Text.Json;

namespace Mamey.Identity.Decentralized.Serialization;

/// <summary>
/// Serializes and deserializes DID Documents using CBOR encoding (RFC 7049).
/// </summary>
public static class CborSerializer
{
    /// <summary>
    /// Serializes a DID Document to CBOR format.
    /// </summary>
    public static byte[] ToCbor(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var writer = new ArrayBufferWriter<byte>();
        var cborWriter = new CborWriter();
        cborWriter.WriteTextString(json);
        return cborWriter.Encode();
    }

    /// <summary>
    /// Deserializes CBOR bytes to a DID Document (via JSON).
    /// </summary>
    public static T FromCbor<T>(byte[] cbor)
    {
        var reader = new CborReader(cbor);
        var json = reader.ReadTextString();
        return JsonSerializer.Deserialize<T>(json);
    }
}