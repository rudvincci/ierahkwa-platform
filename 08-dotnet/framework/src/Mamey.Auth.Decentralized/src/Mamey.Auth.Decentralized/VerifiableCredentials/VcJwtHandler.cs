using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Crypto;

namespace Mamey.Auth.Decentralized.VerifiableCredentials;

/// <summary>
/// Handler for Verifiable Credentials in JWT format
/// </summary>
public class VcJwtHandler
{
    private readonly IKeyGenerator _keyGenerator;
    private readonly ILogger<VcJwtHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the VcJwtHandler class
    /// </summary>
    /// <param name="keyGenerator">The key generator service</param>
    /// <param name="logger">The logger</param>
    public VcJwtHandler(IKeyGenerator keyGenerator, ILogger<VcJwtHandler> logger)
    {
        _keyGenerator = keyGenerator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a JWT from a Verifiable Credential
    /// </summary>
    /// <param name="credential">The verifiable credential</param>
    /// <param name="privateKey">The private key for signing</param>
    /// <param name="algorithm">The signing algorithm</param>
    /// <returns>The JWT string</returns>
    public string CreateJwt(VerifiableCredential credential, byte[] privateKey, string algorithm = "Ed25519")
    {
        if (credential == null)
            throw new ArgumentNullException(nameof(credential));

        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));

        if (!credential.IsValid())
            throw new VcValidationException("Invalid verifiable credential");

        try
        {
            var provider = _keyGenerator.GetProvider(algorithm);
            var publicKey = provider.GenerateKeyPair(privateKey).PublicKey;
            var jwk = provider.PublicKeyToJwk(publicKey);

            var claims = new List<Claim>
            {
                new("vc", JsonSerializer.Serialize(credential)),
                new("iss", credential.Issuer),
                new("sub", credential.CredentialSubject.Id ?? ""),
                new("iat", new DateTimeOffset(credential.IssuanceDate).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("jti", credential.Id ?? Guid.NewGuid().ToString())
            };

            if (credential.ExpirationDate.HasValue)
            {
                claims.Add(new Claim("exp", new DateTimeOffset(credential.ExpirationDate.Value).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = credential.ExpirationDate ?? DateTime.UtcNow.AddYears(1),
                Issuer = credential.Issuer,
                Audience = credential.CredentialSubject.Id,
                SigningCredentials = CreateSigningCredentials(privateKey, algorithm)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create JWT from verifiable credential");
            throw new VcException("Failed to create JWT from verifiable credential", ex);
        }
    }

    /// <summary>
    /// Verifies a JWT and extracts the Verifiable Credential
    /// </summary>
    /// <param name="jwt">The JWT string</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>The verifiable credential</returns>
    public VerifiableCredential VerifyJwt(string jwt, byte[] publicKey, string algorithm = "Ed25519")
    {
        if (string.IsNullOrEmpty(jwt))
            throw new ArgumentException("JWT cannot be null or empty", nameof(jwt));

        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = CreateTokenValidationParameters(publicKey, algorithm);

            var principal = tokenHandler.ValidateToken(jwt, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
                throw new VcValidationException("Invalid JWT token");

            var vcClaim = principal.FindFirst("vc");
            if (vcClaim == null)
                throw new VcValidationException("JWT does not contain verifiable credential claim");

            var credential = JsonSerializer.Deserialize<VerifiableCredential>(vcClaim.Value);
            if (credential == null)
                throw new VcValidationException("Failed to deserialize verifiable credential from JWT");

            if (!credential.IsValid())
                throw new VcValidationException("Invalid verifiable credential in JWT");

            return credential;
        }
        catch (Exception ex) when (!(ex is VcValidationException))
        {
            _logger.LogError(ex, "Failed to verify JWT and extract verifiable credential");
            throw new VcException("Failed to verify JWT and extract verifiable credential", ex);
        }
    }

    /// <summary>
    /// Creates a JWT from a Verifiable Presentation
    /// </summary>
    /// <param name="presentation">The verifiable presentation</param>
    /// <param name="privateKey">The private key for signing</param>
    /// <param name="algorithm">The signing algorithm</param>
    /// <returns>The JWT string</returns>
    public string CreatePresentationJwt(VerifiablePresentation presentation, byte[] privateKey, string algorithm = "Ed25519")
    {
        if (presentation == null)
            throw new ArgumentNullException(nameof(presentation));

        if (privateKey == null)
            throw new ArgumentNullException(nameof(privateKey));

        if (!presentation.IsValid())
            throw new VcValidationException("Invalid verifiable presentation");

        try
        {
            var claims = new List<Claim>
            {
                new("vp", JsonSerializer.Serialize(presentation)),
                new("iss", presentation.Holder ?? ""),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("jti", presentation.Id ?? Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), // Presentations typically have short expiration
                Issuer = presentation.Holder,
                SigningCredentials = CreateSigningCredentials(privateKey, algorithm)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create JWT from verifiable presentation");
            throw new VcException("Failed to create JWT from verifiable presentation", ex);
        }
    }

    /// <summary>
    /// Verifies a JWT and extracts the Verifiable Presentation
    /// </summary>
    /// <param name="jwt">The JWT string</param>
    /// <param name="publicKey">The public key for verification</param>
    /// <param name="algorithm">The verification algorithm</param>
    /// <returns>The verifiable presentation</returns>
    public VerifiablePresentation VerifyPresentationJwt(string jwt, byte[] publicKey, string algorithm = "Ed25519")
    {
        if (string.IsNullOrEmpty(jwt))
            throw new ArgumentException("JWT cannot be null or empty", nameof(jwt));

        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = CreateTokenValidationParameters(publicKey, algorithm);

            var principal = tokenHandler.ValidateToken(jwt, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
                throw new VcValidationException("Invalid JWT token");

            var vpClaim = principal.FindFirst("vp");
            if (vpClaim == null)
                throw new VcValidationException("JWT does not contain verifiable presentation claim");

            var presentation = JsonSerializer.Deserialize<VerifiablePresentation>(vpClaim.Value);
            if (presentation == null)
                throw new VcValidationException("Failed to deserialize verifiable presentation from JWT");

            if (!presentation.IsValid())
                throw new VcValidationException("Invalid verifiable presentation in JWT");

            return presentation;
        }
        catch (Exception ex) when (!(ex is VcValidationException))
        {
            _logger.LogError(ex, "Failed to verify JWT and extract verifiable presentation");
            throw new VcException("Failed to verify JWT and extract verifiable presentation", ex);
        }
    }

    private SigningCredentials CreateSigningCredentials(byte[] privateKey, string algorithm)
    {
        var provider = _keyGenerator.GetProvider(algorithm);
        var keyPair = provider.GenerateKeyPair(privateKey);
        var jwk = provider.PublicKeyToJwk(keyPair.PublicKey);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToBase64String(privateKey)));
        return new SigningCredentials(key, GetSecurityAlgorithm(algorithm));
    }

    private TokenValidationParameters CreateTokenValidationParameters(byte[] publicKey, string algorithm)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToBase64String(publicKey)));
        
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    private string GetSecurityAlgorithm(string algorithm)
    {
        return algorithm switch
        {
            "Ed25519" => SecurityAlgorithms.EcdsaSha256,
            "Secp256k1" => SecurityAlgorithms.EcdsaSha256,
            "RSA" => SecurityAlgorithms.RsaSha256,
            _ => SecurityAlgorithms.HmacSha256
        };
    }
}
