using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Distributed.Configuration;
using Mamey.Identity.Redis.Services;
using Mamey.Identity.Core;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for microservice-to-microservice authentication.
/// </summary>
public class MicroserviceAuthService : IMicroserviceAuthService
{
    private readonly ILogger<MicroserviceAuthService> _logger;
    private readonly DistributedIdentityOptions _options;
    private readonly IRedisTokenCache _tokenCache;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly Dictionary<string, string> _registeredServices;

    public MicroserviceAuthService(
        ILogger<MicroserviceAuthService> logger,
        IOptions<DistributedIdentityOptions> options,
        IRedisTokenCache tokenCache)
    {
        _logger = logger;
        _options = options.Value;
        _tokenCache = tokenCache;
        _tokenHandler = new JwtSecurityTokenHandler();
        _registeredServices = new Dictionary<string, string>();
    }

    public async Task<bool> AuthenticateServiceAsync(string serviceId, string serviceSecret, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if service is registered
            var cacheKey = GetServiceCacheKey(serviceId);
            var serviceDataJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            
            if (string.IsNullOrEmpty(serviceDataJson))
            {
                _logger.LogWarning("Service {ServiceId} not found in registry", serviceId);
                return false;
            }

            var serviceData = JsonSerializer.Deserialize<ServiceData>(serviceDataJson);
            if (serviceData == null)
            {
                _logger.LogWarning("Failed to deserialize service data for {ServiceId}", serviceId);
                return false;
            }

            // Verify secret
            if (serviceData.Secret != serviceSecret)
            {
                _logger.LogWarning("Invalid secret for service {ServiceId}", serviceId);
                return false;
            }

            // Update last heartbeat
            serviceData.LastHeartbeat = DateTime.UtcNow;
            var updatedServiceDataJson = JsonSerializer.Serialize(serviceData);
            await _tokenCache.SetCachedTokenAsync(cacheKey, updatedServiceDataJson);

            _logger.LogDebug("Successfully authenticated service {ServiceId}", serviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating service {ServiceId}", serviceId);
            return false;
        }
    }

    public async Task<string> CreateServiceTokenAsync(string fromServiceId, string toServiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify both services are registered
            if (!await IsServiceRegisteredAsync(fromServiceId) || !await IsServiceRegisteredAsync(toServiceId))
            {
                throw new InvalidOperationException("One or both services are not registered");
            }

            var claims = new List<Claim>
            {
                new Claim("service_id", fromServiceId),
                new Claim("target_service", toServiceId),
                new Claim("token_type", "service"),
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_options.JwtSigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.JwtIssuer,
                audience: _options.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.JwtExpirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = _tokenHandler.WriteToken(token);

            // Cache the service token
            var cacheKey = $"service_token:{fromServiceId}:{toServiceId}";
            await _tokenCache.SetCachedTokenAsync(cacheKey, tokenString);

            _logger.LogDebug("Created service token from {FromServiceId} to {ToServiceId}", fromServiceId, toServiceId);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service token from {FromServiceId} to {ToServiceId}", fromServiceId, toServiceId);
            throw;
        }
    }

    public async Task<bool> ValidateServiceTokenAsync(string token, string expectedServiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = _options.JwtValidationParameters;

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                // Check if it's a service token
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
                if (tokenType != "service")
                {
                    _logger.LogWarning("Token is not a service token");
                    return false;
                }

                // Check target service
                var targetService = jwtToken.Claims.FirstOrDefault(c => c.Type == "target_service")?.Value;
                if (targetService != expectedServiceId)
                {
                    _logger.LogWarning("Token target service {TargetService} does not match expected {ExpectedServiceId}", 
                        targetService, expectedServiceId);
                    return false;
                }

                return true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Service token validation failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating service token");
            return false;
        }
    }

    public async Task<ServiceInfo?> GetServiceInfoAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            var serviceId = jwtToken.Claims.FirstOrDefault(c => c.Type == "service_id")?.Value;
            if (string.IsNullOrEmpty(serviceId))
                return null;

            var cacheKey = GetServiceCacheKey(serviceId);
            var serviceDataJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            
            if (string.IsNullOrEmpty(serviceDataJson))
                return null;

            var serviceData = JsonSerializer.Deserialize<ServiceData>(serviceDataJson);
            if (serviceData == null)
                return null;

            return new ServiceInfo
            {
                ServiceId = serviceData.ServiceId,
                ServiceName = serviceData.ServiceName,
                Version = serviceData.Version,
                Endpoint = serviceData.Endpoint,
                Capabilities = serviceData.Capabilities,
                RegisteredAt = serviceData.RegisteredAt,
                LastHeartbeat = serviceData.LastHeartbeat
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service info from token");
            return null;
        }
    }

    public async Task<bool> RegisterServiceAsync(string serviceId, string serviceSecret, CancellationToken cancellationToken = default)
    {
        try
        {
            var serviceData = new ServiceData
            {
                ServiceId = serviceId,
                ServiceName = serviceId, // Default to service ID if name not provided
                Secret = serviceSecret,
                Version = "1.0.0", // Default version
                Endpoint = string.Empty,
                Capabilities = new List<string>(),
                RegisteredAt = DateTime.UtcNow,
                LastHeartbeat = DateTime.UtcNow
            };

            var serviceDataJson = JsonSerializer.Serialize(serviceData);
            var cacheKey = GetServiceCacheKey(serviceId);
            await _tokenCache.SetCachedTokenAsync(cacheKey, serviceDataJson);

            _logger.LogInformation("Registered service {ServiceId}", serviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering service {ServiceId}", serviceId);
            return false;
        }
    }

    public async Task<bool> UnregisterServiceAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = GetServiceCacheKey(serviceId);
            await _tokenCache.SetCachedTokenAsync(cacheKey, string.Empty); // Set to empty to unregister

            _logger.LogInformation("Unregistered service {ServiceId}", serviceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering service {ServiceId}", serviceId);
            return false;
        }
    }

    private async Task<bool> IsServiceRegisteredAsync(string serviceId)
    {
        try
        {
            var cacheKey = GetServiceCacheKey(serviceId);
            var serviceDataJson = await _tokenCache.GetCachedTokenAsync(cacheKey);
            return !string.IsNullOrEmpty(serviceDataJson);
        }
        catch
        {
            return false;
        }
    }

    private string GetServiceCacheKey(string serviceId) => $"{_options.CacheKeyPrefix}service:{serviceId}";

    private class ServiceData
    {
        public string ServiceId { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public List<string> Capabilities { get; set; } = new();
        public DateTime RegisteredAt { get; set; }
        public DateTime LastHeartbeat { get; set; }
    }
}
