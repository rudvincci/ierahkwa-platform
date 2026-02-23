using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mamey.Security.PostQuantum;
using Mamey.Security.PostQuantum.Interfaces;
using Mamey.Security.PostQuantum.Models;

namespace Mamey.Auth;

/// <summary>
/// Helper service that can generate and validate classical, PQC-only and
/// hybrid JWT tokens based on <see cref="JwtOptions"/> and the configured
/// <see cref="IPQSigner"/> implementation.
/// </summary>
public sealed class PQCAuthenticationService
{
    private readonly JwtOptions _options;
    private readonly IPQSigner _pqSigner;

    public PQCAuthenticationService(JwtOptions options, IPQSigner pqSigner)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _pqSigner = pqSigner ?? throw new ArgumentNullException(nameof(pqSigner));
    }

    /// <summary>
    /// Generates a JWT-like token. When <see cref="JwtOptions.UsePostQuantumSigning"/>
    /// or <see cref="JwtOptions.UseHybridSigning"/> is enabled, the token will
    /// carry a PQC signature and optionally a classical signature, using the
    /// format: <c>header.payload.classicalSig.pqSig</c>.
    /// </summary>
    public PostQuantumJwtToken GenerateToken(
        string subject,
        string? role = null,
        IDictionary<string, string>? claims = null,
        DateTime? utcNow = null)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Subject cannot be null or empty.", nameof(subject));
        }

        var now = utcNow ?? DateTime.UtcNow;
        var expires = now.AddMinutes(_options.ExpirationMinutes);
        var expUnix = new DateTimeOffset(expires).ToUnixTimeSeconds();

        // Header
        var alg = GetAlgorithmName();
        var headerJson = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["alg"] = alg,
            ["typ"] = "JWT"
        });

        // Payload
        var payloadDict = new Dictionary<string, object>
        {
            ["sub"] = subject,
            ["exp"] = expUnix
        };

        if (!string.IsNullOrWhiteSpace(role))
        {
            payloadDict["role"] = role;
        }

        if (!string.IsNullOrWhiteSpace(_options.Issuer))
        {
            payloadDict["iss"] = _options.Issuer;
        }

        if (!string.IsNullOrWhiteSpace(_options.Audience))
        {
            payloadDict["aud"] = _options.Audience;
        }

        if (claims is not null)
        {
            foreach (var (key, value) in claims)
            {
                payloadDict[key] = value;
            }
        }

        var headerSegment = PostQuantumJwtToken.Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var payloadSegment = PostQuantumJwtToken.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payloadDict)));
        var signingInput = Encoding.UTF8.GetBytes($"{headerSegment}.{payloadSegment}");

        string? classicalSig = null;
        string? pqSig = null;

        if (_options.UsePostQuantumSigning)
        {
            var pqSignature = _pqSigner.Sign(signingInput);
            pqSig = PostQuantumJwtToken.Base64UrlEncode(pqSignature);
        }

        // Classical signature (HMAC-SHA256) is always generated when not explicitly
        // disabled by configuration, so that classical 3-part JWTs continue to work
        // and hybrid tokens can require both signatures.
        if (!_options.UsePostQuantumSigning || _options.UseHybridSigning)
        {
            classicalSig = ComputeHmacSha256(signingInput, _options.Secret);
        }

        if (string.IsNullOrEmpty(classicalSig) && string.IsNullOrEmpty(pqSig))
        {
            throw new InvalidOperationException("No signature produced for token. Check JwtOptions configuration.");
        }

        return new PostQuantumJwtToken
        {
            Header = headerSegment,
            Payload = payloadSegment,
            ClassicalSignature = classicalSig,
            PQSignature = pqSig
        };
    }

    /// <summary>
    /// Validates the provided token according to the configured PQC and hybrid
    /// settings. Returns <c>true</c> if the token is structurally valid,
    /// signatures verify, and the token has not expired.
    /// </summary>
    public bool TryValidateToken(string token, out JsonWebTokenPayload? payload)
    {
        payload = null;

        PostQuantumJwtToken parsed;
        try
        {
            parsed = PostQuantumJwtToken.Parse(token);
        }
        catch
        {
            return false;
        }

        var headerJson = PostQuantumJwtToken.Base64UrlDecodeToString(parsed.Header);
        using var headerDoc = JsonDocument.Parse(headerJson);
        if (!headerDoc.RootElement.TryGetProperty("alg", out var algProp))
        {
            return false;
        }

        var alg = algProp.GetString() ?? string.Empty;
        var signingInput = Encoding.UTF8.GetBytes($"{parsed.Header}.{parsed.Payload}");

        var isHybrid = alg.StartsWith("HYBRID", StringComparison.OrdinalIgnoreCase);
        var isPqc = alg.Contains("ML-DSA", StringComparison.OrdinalIgnoreCase);

        var classicalSegment = parsed.ClassicalSignature ?? string.Empty;
        var pqSegment = parsed.PQSignature ?? string.Empty;

        var classicalValid = false;
        var pqValid = false;

        // Classical JWT (3-part) or the classical portion of a hybrid JWT.
        if (!string.IsNullOrEmpty(classicalSegment))
        {
            var expected = ComputeHmacSha256(signingInput, _options.Secret);
            classicalValid = string.Equals(expected, classicalSegment, StringComparison.Ordinal);
        }

        // PQC portion, when present.
        if (!string.IsNullOrEmpty(pqSegment) && isPqc)
        {
            var pqBytes = PostQuantumJwtToken.Base64UrlDecode(pqSegment);
            var publicKey = _options.PQPublicKey is { Length: > 0 }
                ? _options.PQPublicKey
                : _pqSigner.PublicKey;

            pqValid = _pqSigner.Verify(signingInput, pqBytes, publicKey);
        }

        bool signaturesValid;
        if (isHybrid)
        {
            // Hybrid tokens require both signatures to be valid.
            signaturesValid = classicalValid && pqValid;
        }
        else if (isPqc)
        {
            // PQC-only tokens focus on PQC validity.
            signaturesValid = pqValid;
        }
        else
        {
            // Classical-only tokens.
            signaturesValid = classicalValid;
        }

        if (!signaturesValid)
        {
            return false;
        }

        // Decode payload and map to JsonWebTokenPayload.
        var payloadJson = PostQuantumJwtToken.Base64UrlDecodeToString(parsed.Payload);
        using var payloadDoc = JsonDocument.Parse(payloadJson);
        var root = payloadDoc.RootElement;

        if (!root.TryGetProperty("exp", out var expProp))
        {
            return false;
        }

        var expUnix = expProp.GetInt64();
        var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
        if (expDate <= DateTime.UtcNow)
        {
            return false;
        }

        var subject = root.TryGetProperty("sub", out var subProp) ? subProp.GetString() : null;
        var role = root.TryGetProperty("role", out var roleProp) ? roleProp.GetString() : null;

        var claimsDict = new Dictionary<string, IEnumerable<string>>();
        foreach (var property in root.EnumerateObject())
        {
            if (property.Name is "sub" or "exp" or "role")
            {
                continue;
            }

            claimsDict[property.Name] = new[] { property.Value.ToString() };
        }

        payload = new JsonWebTokenPayload
        {
            Subject = subject ?? string.Empty,
            Role = role ?? string.Empty,
            Expires = expUnix,
            Claims = claimsDict
        };

        return true;
    }

    private string GetAlgorithmName()
    {
        if (_options.UsePostQuantumSigning)
        {
            if (_options.UseHybridSigning)
            {
                // Logical algorithm profile for hybrid tokens.
                return "HYBRID-RSA-MLDSA65";
            }

            return _options.SignatureAlgorithm switch
            {
                SignatureAlgorithm.ML_DSA_44 => "ML-DSA-44",
                SignatureAlgorithm.ML_DSA_65 => "ML-DSA-65",
                SignatureAlgorithm.ML_DSA_87 => "ML-DSA-87",
                _ => "ML-DSA-65"
            };
        }

        // Classical-only tokens.
        return "HS256";
    }

    private static string ComputeHmacSha256(ReadOnlySpan<byte> data, string secret)
    {
        if (string.IsNullOrEmpty(secret))
        {
            throw new InvalidOperationException("JwtOptions.Secret must be configured for classical signing.");
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(data.ToArray());
        return PostQuantumJwtToken.Base64UrlEncode(hash);
    }
}


