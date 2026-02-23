using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Auth.DecentralizedIdentifiers.Serialization;

/// <summary>
/// Provides serialization and deserialization for JWS/JWT for signing and protecting DID Documents and VCs.
/// </summary>
public static class JwsSerializer
{
    /// <summary>
    /// Signs a JSON payload and returns a compact JWS.
    /// </summary>
    public static string Sign(string payloadJson, byte[] key, string alg = SecurityAlgorithms.EcdsaSha256)
    {
        var securityKey = new SymmetricSecurityKey(key);
        var creds = new SigningCredentials(securityKey, alg);
        
        var handler = new JwtSecurityTokenHandler();
        var claimsIdentity = new ClaimsIdentity();
        claimsIdentity.AddClaim(new Claim("payload", payloadJson));
        var token = handler.CreateJwtSecurityToken(
            subject: claimsIdentity,
            signingCredentials: creds
        );
        return handler.WriteToken(token);
    }

    /// <summary>
    /// Verifies a JWS and extracts the payload JSON.
    /// </summary>
    public static string Verify(string jws, byte[] key, string alg = SecurityAlgorithms.EcdsaSha256)
    {
        var handler = new JwtSecurityTokenHandler();
        var validation = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidAlgorithms = new[] { alg }
        };
        handler.ValidateToken(jws, validation, out var validatedToken);
        var jwt = (JwtSecurityToken)validatedToken;
        return jwt.Claims.First(c => c.Type == "payload").Value;
    }
}