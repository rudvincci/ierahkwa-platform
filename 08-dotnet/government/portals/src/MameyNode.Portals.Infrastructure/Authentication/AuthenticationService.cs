using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Mamey.FWID.Identities.BlazorWasm.Services;
using Mamey.FWID.DIDs.BlazorWasm.Services;
using Mamey.Auth;

namespace MameyNode.Portals.Infrastructure.Authentication;

/// <summary>
/// Multi-method authentication service implementation
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly IDIDService _didService;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly PQCAuthenticationHandler _pqcHandler;

    public AuthenticationService(
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,
        IDIDService didService,
        ILogger<AuthenticationService> logger,
        IConfiguration configuration,
        PQCAuthenticationHandler pqcHandler)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _didService = didService;
        _logger = logger;
        _configuration = configuration;
        _pqcHandler = pqcHandler;
    }

    public async Task<AuthenticationResult> AuthenticateWithJwtAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTimeOffset.UtcNow;
            var allowed = _pqcHandler.IsTokenAllowed(token, now, out var isPqcToken, out var isHybridToken);
            if (!allowed)
            {
                _logger.LogWarning("JWT/PQC policy rejected token.");
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Token does not satisfy PQC authentication policy."
                };
            }

            AuthenticatedUser user;

            var segments = token.Split('.');
            if (segments.Length == 4 || isPqcToken || isHybridToken)
            {
                // PQC / hybrid token using extended JWT format: header.payload.classicalSig.pqSig
                var pqToken = PostQuantumJwtToken.Parse(token);
                var payloadJson = PostQuantumJwtToken.Base64UrlDecodeToString(pqToken.Payload);
                using var doc = JsonDocument.Parse(payloadJson);
                var root = doc.RootElement;

                var userId = root.TryGetProperty("sub", out var subProp) ? subProp.GetString() ?? string.Empty : string.Empty;
                var email = root.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? string.Empty : string.Empty;
                var name = root.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? string.Empty : string.Empty;
                var role = root.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? string.Empty : string.Empty;

                var roles = new List<string>();
                if (!string.IsNullOrEmpty(role))
                {
                    roles.Add(role);
                }

                var claims = new Dictionary<string, string>();
                foreach (var property in root.EnumerateObject())
                {
                    if (property.Name is "sub" or "exp" or "role" or "iss" or "aud")
                    {
                        continue;
                    }
                    claims[property.Name] = property.Value.ToString();
                }

                user = new AuthenticatedUser
                {
                    UserId = userId,
                    Email = email,
                    Name = name,
                    Roles = roles,
                    AuthenticationMethod = "JWT-PQC",
                    Claims = claims
                };
            }
            else
            {
                // Classical JWT (3-part) path – use standard handler to read claims.
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                user = new AuthenticatedUser
                {
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                    Name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty,
                    Roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                    AuthenticationMethod = "JWT",
                    Claims = jwtToken.Claims.ToDictionary(c => c.Type, c => c.Value)
                };
            }

            _logger.LogInformation("User authenticated with JWT: {UserId}", user.UserId);
            
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "JWT authentication failed");
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid JWT token"
            };
        }
    }

    public async Task<AuthenticationResult> AuthenticateWithAzureAdAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // In real implementation, would validate Azure AD token
            // For now, parse token and extract claims
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            
            var user = new AuthenticatedUser
            {
                UserId = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value ?? string.Empty,
                Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
                Name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
                Roles = token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList(),
                AuthenticationMethod = "AzureAD",
                Claims = token.Claims.ToDictionary(c => c.Type, c => c.Value)
            };

            _logger.LogInformation("User authenticated with Azure AD: {UserId}", user.UserId);
            
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure AD authentication failed");
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "Invalid Azure AD token"
            };
        }
    }

    public async Task<AuthenticationResult> AuthenticateWithFutureWampumIdAsync(string did, string proof, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify DID and proof using FutureWampumID services
            var didDocument = await _didService.ResolveDIDAsync(did, cancellationToken);
            if (didDocument == null)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "DID not found"
                };
            }

            // Verify proof (simplified - real implementation would use ZKP verification)
            var identity = await _identityService.GetIdentityByDIDAsync(did, cancellationToken);
            if (identity == null)
            {
                return new AuthenticationResult
                {
                    IsSuccess = false,
                    ErrorMessage = "Identity not found"
                };
            }

            var user = new AuthenticatedUser
            {
                UserId = did,
                Email = identity.Email ?? string.Empty,
                Name = identity.Name ?? string.Empty,
                Roles = identity.Roles?.ToList() ?? new List<string>(),
                AuthenticationMethod = "FutureWampumID",
                Claims = new Dictionary<string, string>
                {
                    ["did"] = did,
                    ["proof"] = proof
                }
            };

            _logger.LogInformation("User authenticated with FutureWampumID: {DID}", did);
            
            return new AuthenticationResult
            {
                IsSuccess = true,
                User = user
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FutureWampumID authentication failed");
            return new AuthenticationResult
            {
                IsSuccess = false,
                ErrorMessage = "FutureWampumID authentication failed"
            };
        }
    }

    public async Task<AuthenticatedUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var claims = httpContext.User.Claims.ToList();
            return new AuthenticatedUser
            {
                UserId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                Name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty,
                Roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                AuthenticationMethod = httpContext.User.Identity.AuthenticationType ?? "Unknown",
                Claims = claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }
        return null;
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            await httpContext.SignOutAsync();
            _logger.LogInformation("User signed out");
        }
    }
}


