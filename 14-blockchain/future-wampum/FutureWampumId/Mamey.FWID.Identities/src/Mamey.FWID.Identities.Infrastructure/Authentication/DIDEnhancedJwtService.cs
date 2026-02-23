using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.FWID.Identities.Infrastructure.Authentication;

/// <summary>
/// Service for generating JWT tokens with DID claims.
/// Includes DID-related claims when identity has linked DIDs.
/// 
/// TDD Reference: Lines 1594-1703 (Identity), Lines 2207-2336 (DID)
/// BDD Reference: Lines 326-434
/// </summary>
internal sealed class DIDEnhancedJwtService : IDIDEnhancedJwtService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IDIDAuthenticationService _didAuthService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DIDEnhancedJwtService> _logger;
    private readonly JwtTokenOptions _options;
    private readonly SymmetricSecurityKey _signingKey;

    public DIDEnhancedJwtService(
        IIdentityRepository identityRepository,
        IDIDAuthenticationService didAuthService,
        IMemoryCache cache,
        ILogger<DIDEnhancedJwtService> logger,
        IConfiguration configuration)
    {
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _didAuthService = didAuthService ?? throw new ArgumentNullException(nameof(didAuthService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options = new JwtTokenOptions();
        configuration.GetSection("jwt").Bind(_options);

        // Initialize signing key
        var keyBytes = !string.IsNullOrEmpty(_options.SecretKey)
            ? Encoding.UTF8.GetBytes(_options.SecretKey)
            : RandomNumberGenerator.GetBytes(64);
        _signingKey = new SymmetricSecurityKey(keyBytes);
    }

    /// <inheritdoc />
    public async Task<DIDEnhancedTokenResult> GenerateTokenAsync(
        Guid identityId,
        string? primaryDid = null,
        Dictionary<string, string>? additionalClaims = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating DID-enhanced JWT: IdentityId={IdentityId}", identityId);

        try
        {
            var identity = await _identityRepository.GetAsync(
                new Domain.Entities.IdentityId(identityId),
                cancellationToken);

            if (identity == null)
            {
                return new DIDEnhancedTokenResult
                {
                    Success = false,
                    ErrorMessage = "Identity not found"
                };
            }

            // Get linked DIDs
            var linkedDids = await _didAuthService.GetLinkedDIDsAsync(identityId, cancellationToken);
            var primary = primaryDid ?? linkedDids.FirstOrDefault(d => d.IsPrimary)?.DID;

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identityId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("identity_id", identityId.ToString())
            };

            // Add DID claims if available
            DIDClaimsInfo? didClaimsInfo = null;
            if (!string.IsNullOrEmpty(primary))
            {
                var didMethod = ExtractDIDMethod(primary);
                var linkedDidInfo = linkedDids.FirstOrDefault(d => d.DID == primary);

                claims.Add(new Claim("did", primary));
                claims.Add(new Claim("did_method", didMethod));
                claims.Add(new Claim("did_verified", (linkedDidInfo?.IsBlockchainVerified ?? false).ToString().ToLower()));

                if (linkedDids.Count > 1)
                {
                    claims.Add(new Claim("linked_dids_count", linkedDids.Count.ToString()));
                }

                didClaimsInfo = new DIDClaimsInfo
                {
                    DID = primary,
                    DIDMethod = didMethod,
                    DIDVerified = linkedDidInfo?.IsBlockchainVerified ?? false,
                    BlockchainVerified = linkedDidInfo?.IsBlockchainVerified ?? false,
                    LinkedDIDs = linkedDids.Select(d => d.DID).ToList(),
                    VerifiedAt = linkedDidInfo?.LastUsedAt
                };
            }

            // Add additional claims
            if (additionalClaims != null)
            {
                foreach (var claim in additionalClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            // Add roles (placeholder - actual implementation would get from identity)
            claims.Add(new Claim(ClaimTypes.Role, "User"));

            // Generate token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_options.AccessTokenExpirationHours),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();
            
            // Cache refresh token
            var refreshCacheKey = $"refresh_token:{refreshToken}";
            _cache.Set(refreshCacheKey, identityId, TimeSpan.FromDays(_options.RefreshTokenExpirationDays));

            _logger.LogInformation(
                "DID-enhanced JWT generated: IdentityId={IdentityId}, HasDID={HasDID}",
                identityId, !string.IsNullOrEmpty(primary));

            return new DIDEnhancedTokenResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = tokenDescriptor.Expires,
                DIDClaims = didClaimsInfo
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating DID-enhanced JWT: IdentityId={IdentityId}", identityId);
            return new DIDEnhancedTokenResult
            {
                Success = false,
                ErrorMessage = $"Token generation error: {ex.Message}"
            };
        }
    }

    /// <inheritdoc />
    public async Task<DIDEnhancedTokenResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"refresh_token:{refreshToken}";
        
        if (!_cache.TryGetValue(cacheKey, out Guid identityId))
        {
            return new DIDEnhancedTokenResult
            {
                Success = false,
                ErrorMessage = "Invalid or expired refresh token"
            };
        }

        // Invalidate old refresh token
        _cache.Remove(cacheKey);

        // Generate new tokens
        return await GenerateTokenAsync(identityId, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public DIDClaimsInfo? ExtractDIDClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var didClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "did");
            if (didClaim == null)
                return null;

            return new DIDClaimsInfo
            {
                DID = didClaim.Value,
                DIDMethod = jwtToken.Claims.FirstOrDefault(c => c.Type == "did_method")?.Value,
                DIDVerified = bool.TryParse(
                    jwtToken.Claims.FirstOrDefault(c => c.Type == "did_verified")?.Value,
                    out var verified) && verified,
                BlockchainVerified = verified
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting DID claims from token");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateDIDClaimsAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var claims = ExtractDIDClaims(token);
        if (claims?.DID == null)
            return true; // No DID claims to validate

        // Verify the DID still exists and is valid
        // In production, this would verify against the DID service
        return true; // Placeholder
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[48];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string ExtractDIDMethod(string did)
    {
        var parts = did.Split(':');
        return parts.Length >= 2 ? parts[1] : "unknown";
    }
}

/// <summary>
/// JWT token generation options.
/// </summary>
public class JwtTokenOptions
{
    /// <summary>JWT secret key.</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>JWT issuer.</summary>
    public string Issuer { get; set; } = "https://identity.futurewampum.io";

    /// <summary>JWT audience.</summary>
    public string Audience { get; set; } = "https://api.futurewampum.io";

    /// <summary>Access token expiration hours.</summary>
    public int AccessTokenExpirationHours { get; set; } = 1;

    /// <summary>Refresh token expiration days.</summary>
    public int RefreshTokenExpirationDays { get; set; } = 30;
}
