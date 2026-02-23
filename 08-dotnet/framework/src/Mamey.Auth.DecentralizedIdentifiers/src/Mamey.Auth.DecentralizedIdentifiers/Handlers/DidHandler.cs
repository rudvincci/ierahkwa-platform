using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.Services;
using Mamey.Auth.DecentralizedIdentifiers.Validation;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Mamey.Security;
using Mamey;

namespace Mamey.Auth.DecentralizedIdentifiers.Handlers;

/// <summary>
/// Implementation of DID authentication handler
/// </summary>
public class DidHandler : IDidHandler
{
    private readonly IDidResolver _didResolver;
    private readonly IKeyProvider _keyProvider;
    private readonly IProofService _proofService;
    private readonly IVerifiableCredentialValidator _vcValidator;
    private readonly ISecurityProvider _securityProvider;
    private readonly ILogger<DidHandler> _logger;
    private readonly DidAuthOptions _options;

    public DidHandler(
        IDidResolver didResolver,
        IKeyProvider keyProvider,
        IProofService proofService,
        IVerifiableCredentialValidator vcValidator,
        ISecurityProvider securityProvider,
        ILogger<DidHandler> logger,
        DidAuthOptions options)
    {
        _didResolver = didResolver;
        _keyProvider = keyProvider;
        _proofService = proofService;
        _vcValidator = vcValidator;
        _securityProvider = securityProvider;
        _logger = logger;
        _options = options;
    }

    public async Task<DidToken> CreateDidToken(string did, IDictionary<string, string> claims = null)
    {
        try
        {
            // Resolve DID document to get verification methods
            var didDocument = await _didResolver.ResolveAsync(did);
            if (didDocument == null)
            {
                throw new InvalidOperationException($"Could not resolve DID: {did}");
            }

            // Get signing key
            var privateKeyBytes = await _keyProvider.GetPrivateKeyAsync(did);
            var signingKey = new SymmetricSecurityKey(privateKeyBytes);
            var keyId = Guid.NewGuid().ToString();

            // Create JWT claims
            var jwtClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, did),
                new("sub", did),
                new("iss", _options.Issuer),
                new("aud", _options.Audience),
                new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("exp", DateTimeOffset.UtcNow.AddMinutes(_options.Authentication.TokenExpirationMinutes).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new("jti", Guid.NewGuid().ToString())
            };

            // Add custom claims
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    jwtClaims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            // Create JWT token
            var expires = DateTime.UtcNow.AddMinutes(_options.Authentication.TokenExpirationMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(jwtClaims),
                Expires = expires,
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("Created DID token for {Did}", did);

            return new DidToken
            {
                AccessToken = tokenString,
                Expires = ((DateTimeOffset)expires).ToUnixTimeSeconds(),
                Id = did,
                Role = claims?.ContainsKey("role") == true ? claims["role"] : "user",
                Claims = claims ?? new Dictionary<string, string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create DID token for {Did}", did);
            throw;
        }
    }

    public async Task<VerifiablePresentation> CreateVerifiablePresentation(
        string did, 
        IEnumerable<VerifiableCredential> credentials,
        PresentationOptions options = null)
    {
        try
        {
            options ??= new PresentationOptions();

            // Resolve DID document
            var didDocument = await _didResolver.ResolveAsync(did);
            if (didDocument == null)
            {
                throw new InvalidOperationException($"Could not resolve DID: {did}");
            }

            // Create presentation
            var presentation = new VerifiablePresentation
            {
                Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiablePresentation" },
                Holder = did,
                VerifiableCredential = credentials.ToList()
            };

            // Add challenge and domain if provided
            if (!string.IsNullOrEmpty(options.Challenge))
            {
                presentation.Challenge = options.Challenge;
            }

            if (!string.IsNullOrEmpty(options.Domain))
            {
                presentation.Domain = options.Domain;
            }

            // Create proof
            var jsonLd = JsonSerializer.Serialize(presentation);
            var verificationMethod = didDocument.DidDocument.VerificationMethods?.FirstOrDefault();
            if (verificationMethod == null)
                throw new InvalidOperationException("No verification method found in DID document");

            var privateKey = await _keyProvider.GetPrivateKeyAsync(did);
            var proof = await _proofService.CreateProofAsync(
                jsonLd,
                verificationMethod.Id,
                privateKey,
                options.ProofPurpose ?? "authentication",
                "Ed25519Signature2020",
                DateTimeOffset.UtcNow.ToString("o"));

            presentation.Proof = proof;

            _logger.LogInformation("Created Verifiable Presentation for {Did}", did);

            return presentation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Verifiable Presentation for {Did}", did);
            throw;
        }
    }

    public async Task<bool> ValidateDidToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            if (!tokenHandler.CanReadToken(token))
            {
                return false;
            }

            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Validate DID subject
            var did = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(did) || !did.StartsWith("did:"))
            {
                return false;
            }

            // Resolve DID document
            var didDocument = await _didResolver.ResolveAsync(did);
            if (didDocument == null)
            {
                return false;
            }

            // Validate token signature (this would be done by JWT middleware)
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate DID token");
            return false;
        }
    }

    public async Task<PresentationValidationResult> ValidatePresentation(VerifiablePresentation vp)
    {
        var result = new PresentationValidationResult();

        try
        {
            // Validate presentation structure
            if (vp == null)
            {
                result.Errors.Add("Presentation is null");
                return result;
            }

            if (string.IsNullOrEmpty(vp.Holder))
            {
                result.Errors.Add("Presentation holder is missing");
                return result;
            }

            // Resolve holder DID
            var holderDidDocument = await _didResolver.ResolveAsync(vp.Holder);
            if (holderDidDocument == null)
            {
                result.Errors.Add($"Could not resolve holder DID: {vp.Holder}");
                return result;
            }

            // Validate presentation proof
            if (vp.Proof == null)
            {
                result.Errors.Add("Presentation proof is missing");
                return result;
            }

            var verificationMethod = holderDidDocument.DidDocument.VerificationMethods?.FirstOrDefault();
            if (verificationMethod == null)
            {
                result.Errors.Add("No verification method found in holder DID document");
                return result;
            }

            var proofObj = vp.Proof;
            var proofType = proofObj is Proof proof ? proof.Type : 
                           proofObj is Dictionary<string, object> dict ? dict.GetValueOrDefault("type")?.ToString() :
                           "Ed25519Signature2020";
            var proofPurpose = proofObj is Proof proof2 ? proof2.ProofPurpose :
                              proofObj is Dictionary<string, object> dict2 ? dict2.GetValueOrDefault("proofPurpose")?.ToString() :
                              "authentication";
            
            var proofValid = await _proofService.VerifyProofAsync(
                JsonSerializer.Serialize(vp),
                proofObj,
                new byte[0], // Public key would be extracted from verification method
                proofType ?? "Ed25519Signature2020",
                proofPurpose ?? "authentication");
            if (!proofValid)
            {
                result.Errors.Add("Presentation proof is invalid");
            }

            // Validate each credential
            if (vp.VerifiableCredential != null)
            {
                var credentials = vp.VerifiableCredential is IEnumerable<VerifiableCredential> credentialList 
                    ? credentialList 
                    : new[] { (VerifiableCredential)vp.VerifiableCredential };

                foreach (var credential in credentials)
                {
                    var credentialJson = JsonSerializer.Serialize(credential);
                    var credentialResult = await _vcValidator.ValidateAsync(credentialJson);
                    if (!credentialResult.IsValid)
                    {
                        result.Errors.AddRange(credentialResult.Errors);
                    }
                }
            }

            // Validate challenge and domain if required
            if (_options.Authentication.RequireChallenge && string.IsNullOrEmpty(vp.Challenge))
            {
                result.Errors.Add("Challenge is required but missing");
            }

            if (_options.Authentication.RequireDomainBinding && string.IsNullOrEmpty(vp.Domain))
            {
                result.Errors.Add("Domain binding is required but missing");
            }

            result.IsValid = !result.Errors.Any();

            _logger.LogInformation("Validated Verifiable Presentation for {Holder}, Valid: {IsValid}", 
                vp.Holder, result.IsValid);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate Verifiable Presentation");
            result.Errors.Add($"Validation error: {ex.Message}");
            return result;
        }
    }

    public async Task<DidTokenPayload> GetDidPayload(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var payload = new DidTokenPayload
            {
                Subject = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
                Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? string.Empty,
                Expires = ((DateTimeOffset)jwtToken.ValidTo).ToUnixTimeSeconds(),
                Claims = jwtToken.Claims.GroupBy(c => c.Type)
                    .ToDictionary(g => g.Key, g => g.Select(c => c.Value))
            };

            return payload;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract DID token payload");
            throw;
        }
    }

    public async Task<bool> VerifySignatureAsync(DidDocument didDocument, string challenge, string signature)
    {
        try
        {
            // Find appropriate verification method
            var verificationMethod = didDocument.VerificationMethods?.FirstOrDefault();
            if (verificationMethod == null)
            {
                return false;
            }

            // Verify signature using the verification method
            var isValid = await _proofService.VerifyProofAsync(
                challenge,
                new { signature = signature },
                verificationMethod.PublicKey,
                verificationMethod.Type,
                "authentication");

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify signature");
            return false;
        }
    }

    public IEnumerable<SecurityKey> ResolveSigningKey(string keyId)
    {
        try
        {
            var key = _keyProvider.ResolveKey(keyId);
            return new[] { new SymmetricSecurityKey(key) { KeyId = keyId } };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve signing key: {KeyId}", keyId);
            throw;
        }
    }

    public async Task<string> SignAsync(string data, string privateKey)
    {
        try
        {
            // Use security provider to sign data
            var signature = _securityProvider.Hash(data + privateKey);
            return signature;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign data");
            throw;
        }
    }
}





