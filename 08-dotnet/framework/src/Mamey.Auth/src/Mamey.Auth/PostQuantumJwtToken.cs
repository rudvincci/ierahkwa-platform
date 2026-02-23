using System.Text;
using System.Text.Json;

namespace Mamey.Auth;

/// <summary>
/// Represents a JWT-like token that may carry both classical and
/// post-quantum signatures. The underlying string representation is:
/// - Classical: header.payload.classicalSig
/// - PQC/Hybrid: header.payload.classicalSig.pqSig
/// where each segment is base64url-encoded.
/// </summary>
public sealed class PostQuantumJwtToken
{
    /// <summary>
    /// Base64url-encoded header JSON.
    /// </summary>
    public string Header { get; init; } = string.Empty;

    /// <summary>
    /// Base64url-encoded payload JSON.
    /// </summary>
    public string Payload { get; init; } = string.Empty;

    /// <summary>
    /// Base64url-encoded classical signature segment (e.g. HMAC).
    /// For PQC-only tokens this may be null or empty.
    /// </summary>
    public string? ClassicalSignature { get; init; }

    /// <summary>
    /// Base64url-encoded post-quantum signature segment (e.g. ML-DSA).
    /// </summary>
    public string? PQSignature { get; init; }

    /// <summary>
    /// Returns the compact token string representation.
    /// </summary>
    public string ToTokenString()
    {
        if (string.IsNullOrEmpty(Header) || string.IsNullOrEmpty(Payload))
        {
            throw new InvalidOperationException("Header and Payload must be set before generating a token string.");
        }

        if (string.IsNullOrEmpty(ClassicalSignature) && string.IsNullOrEmpty(PQSignature))
        {
            // Classical 3-part JWT.
            throw new InvalidOperationException("At least one signature (classical or PQC) must be present.");
        }

        if (string.IsNullOrEmpty(PQSignature))
        {
            // Classical-only.
            return string.Join('.', Header, Payload, ClassicalSignature ?? string.Empty);
        }

        // PQC or hybrid.
        return string.Join('.', Header, Payload, ClassicalSignature ?? string.Empty, PQSignature);
    }

    /// <summary>
    /// Parses a classical (3-part) or PQC/hybrid (4-part) JWT string into
    /// a <see cref="PostQuantumJwtToken"/>. Does not perform any
    /// cryptographic validation.
    /// </summary>
    public static PostQuantumJwtToken Parse(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        var segments = token.Split('.');
        if (segments.Length is < 3 or > 4)
        {
            throw new ArgumentException("Token must have 3 (classical) or 4 (PQC/hybrid) segments.", nameof(token));
        }

        return new PostQuantumJwtToken
        {
            Header = segments[0],
            Payload = segments[1],
            ClassicalSignature = segments.Length >= 3 ? segments[2] : null,
            PQSignature = segments.Length == 4 ? segments[3] : null
        };
    }

    /// <summary>
    /// Decodes the header JSON document.
    /// </summary>
    public JsonDocument GetHeaderJson() => JsonDocument.Parse(Base64UrlDecodeToString(Header));

    /// <summary>
    /// Decodes the payload JSON document.
    /// </summary>
    public JsonDocument GetPayloadJson() => JsonDocument.Parse(Base64UrlDecodeToString(Payload));

    internal static string Base64UrlEncode(byte[] data)
    {
        var base64 = Convert.ToBase64String(data);
        return base64.TrimEnd('=')
                     .Replace('+', '-')
                     .Replace('/', '_');
    }

    internal static byte[] Base64UrlDecode(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Array.Empty<byte>();
        }

        var padded = value.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }

        return Convert.FromBase64String(padded);
    }

    public static string Base64UrlDecodeToString(string value)
        => Encoding.UTF8.GetString(Base64UrlDecode(value));
}


