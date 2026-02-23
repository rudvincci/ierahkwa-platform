using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Decentralized.Abstractions;
using Newtonsoft.Json.Linq;

namespace Mamey.Identity.Decentralized;

/// <summary>
/// Extension methods for converting DID Verification Methods into SecurityKeys for JWT/JWS validation.
/// </summary>
public static class VerificationMethodSecurityKeyExtensions
{
    /// <summary>
    /// Converts an <see cref="IDidVerificationMethod"/> into a <see cref="SecurityKey"/>.
    /// </summary>
    public static SecurityKey ToSecurityKey(this IDidVerificationMethod method)
    {
        if (method == null) throw new ArgumentNullException(nameof(method));

        // 1. JWK
        if (method.PublicKeyJwk != null && method.PublicKeyJwk.Count > 0)
        {
            var jwk = JObject.FromObject(method.PublicKeyJwk).ToString();
            return new JsonWebKey(jwk);
        }

        // 2. Multibase
        if (!string.IsNullOrWhiteSpace(method.PublicKeyMultibase))
        {
            var pub = Crypto.MultibaseUtil.Decode(method.PublicKeyMultibase);
            if (method.Type?.Contains("Ed25519", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new NotSupportedException("Ed25519SecurityKey not available in .NET 9. Use a compatible library (e.g., Dahomey.IdentityModel.Tokens.Ed25519) or custom SecurityKey implementation.");
            }
            else if (method.Type?.Contains("secp256k1", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new NotSupportedException("Secp256k1SecurityKey not available in .NET 9. Use a compatible library or custom implementation.");
            }
        }

        // 3. Base58
        if (!string.IsNullOrWhiteSpace(method.PublicKeyBase58))
        {
            var pub = Crypto.Base58Util.Decode(method.PublicKeyBase58);
            if (method.Type?.Contains("Ed25519", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new NotSupportedException("Ed25519SecurityKey not available in .NET 9. Use a compatible library or custom implementation.");
            }
            else if (method.Type?.Contains("secp256k1", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new NotSupportedException("Secp256k1SecurityKey not available in .NET 9. Use a compatible library or custom implementation.");
            }
        }

        throw new NotSupportedException("No supported key material found for conversion to SecurityKey.");
    }
}