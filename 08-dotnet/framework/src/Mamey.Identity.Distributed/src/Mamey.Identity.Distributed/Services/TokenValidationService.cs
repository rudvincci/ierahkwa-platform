using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Distributed.Configuration;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for validating tokens in distributed scenarios.
/// </summary>
public class TokenValidationService : ITokenValidationService
{
    private readonly ILogger<TokenValidationService> _logger;
    private readonly DistributedIdentityOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenValidationService(
        ILogger<TokenValidationService> logger,
        IOptions<DistributedIdentityOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<bool> ValidateJwtTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var validationParameters = _options.JwtValidationParameters;

            try
            {
                _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogDebug(ex, "JWT token validation failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating JWT token");
            return false;
        }
    }

    public async Task<bool> ValidateServiceTokenAsync(string token, string expectedServiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await ValidateJwtTokenAsync(token, cancellationToken))
                return false;

            var jwtToken = _tokenHandler.ReadJwtToken(token);
            
            // Check if it's a service token
            var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
            if (tokenType != "service")
            {
                _logger.LogDebug("Token is not a service token");
                return false;
            }

            // Check target service
            var targetService = jwtToken.Claims.FirstOrDefault(c => c.Type == "target_service")?.Value;
            if (targetService != expectedServiceId)
            {
                _logger.LogDebug("Token target service {TargetService} does not match expected {ExpectedServiceId}", 
                    targetService, expectedServiceId);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating service token");
            return false;
        }
    }

    public async Task<bool> ValidateDistributedTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await ValidateJwtTokenAsync(token, cancellationToken))
                return false;

            var jwtToken = _tokenHandler.ReadJwtToken(token);
            
            // Check if it's a distributed token (has service_id claim)
            var serviceId = jwtToken.Claims.FirstOrDefault(c => c.Type == "service_id")?.Value;
            if (string.IsNullOrEmpty(serviceId))
            {
                _logger.LogDebug("Token is not a distributed token (missing service_id claim)");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating distributed token");
            return false;
        }
    }

    public async Task<IEnumerable<Claim>?> GetClaimsAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await ValidateJwtTokenAsync(token, cancellationToken))
                return null;

            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting claims from token");
            return null;
        }
    }

    public async Task<bool> IsTokenExpiredAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo <= DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking token expiration");
            return true; // Assume expired if we can't check
        }
    }

    public async Task<bool> ValidateIssuerAsync(string token, string expectedIssuer, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Issuer == expectedIssuer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token issuer");
            return false;
        }
    }

    public async Task<bool> ValidateAudienceAsync(string token, string expectedAudience, CancellationToken cancellationToken = default)
    {
        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.Audiences.Contains(expectedAudience);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token audience");
            return false;
        }
    }
}


































