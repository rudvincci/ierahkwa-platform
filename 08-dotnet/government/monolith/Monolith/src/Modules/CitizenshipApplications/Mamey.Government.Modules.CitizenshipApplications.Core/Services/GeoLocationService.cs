using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Persistence.Redis;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal sealed class GeoLocationService : IGeoLocationService
{
    private const string DefaultLookupUrlTemplate = "http://ip-api.com/json/{0}?fields=status,message,country,countryCode,regionName,city,zip,lat,lon,timezone";
    private const string CachePrefix = "citizenship-applications:geo:";
    private static readonly TimeSpan DefaultCacheTtl = TimeSpan.FromHours(24);
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeoLocationService> _logger;
    private readonly ICache _cache;
    private readonly GeoLocationOptions _options;

    public GeoLocationService(
        HttpClient httpClient,
        ILogger<GeoLocationService> logger,
        ICache cache,
        IOptions<GeoLocationOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cache = cache;
        _options = options.Value ?? new GeoLocationOptions();
    }

    public async Task<GeoLocationResult?> LookupAsync(
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return null;
        }

        if (!IPAddress.TryParse(ipAddress, out var ip) || IsPrivateIp(ip))
        {
            return null;
        }

        var cacheKey = $"{CachePrefix}{ipAddress}:{HashUserAgent(userAgent)}";
        try
        {
            var cached = await _cache.GetAsync<GeoLocationResult>(cacheKey);
            if (cached != null)
            {
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read geolocation cache for IP {IpAddress}", ipAddress);
        }

        try
        {
            var urlTemplate = string.IsNullOrWhiteSpace(_options.LookupUrlTemplate)
                ? DefaultLookupUrlTemplate
                : _options.LookupUrlTemplate;
            var url = string.Format(urlTemplate, ipAddress);
            var response = await _httpClient.GetFromJsonAsync<IpApiResponse>(url, cancellationToken);
            if (response is null || !string.Equals(response.Status, "success", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var result = new GeoLocationResult(
                response.Country,
                response.CountryCode,
                response.RegionName,
                response.City,
                response.Zip,
                response.Timezone,
                response.Lat,
                response.Lon);
            try
            {
                var ttl = _options.CacheTtlMinutes.HasValue
                    ? TimeSpan.FromMinutes(_options.CacheTtlMinutes.Value)
                    : DefaultCacheTtl;
                await _cache.SetAsync(cacheKey, result, ttl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to write geolocation cache for IP {IpAddress}", ipAddress);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to resolve geolocation for IP {IpAddress}", ipAddress);
            return null;
        }
    }

    private static bool IsPrivateIp(IPAddress ipAddress)
    {
        if (IPAddress.IsLoopback(ipAddress))
        {
            return true;
        }

        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            var bytes = ipAddress.GetAddressBytes();
            return bytes[0] switch
            {
                10 => true,
                127 => true,
                172 => bytes[1] >= 16 && bytes[1] <= 31,
                192 => bytes[1] == 168,
                _ => false
            };
        }

        if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return ipAddress.IsIPv6LinkLocal || ipAddress.IsIPv6SiteLocal || ipAddress.IsIPv6Multicast;
        }

        return false;
    }

    private static string HashUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "na";
        }

        var bytes = Encoding.UTF8.GetBytes(userAgent);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private sealed class IpApiResponse
    {
        public string? Status { get; set; }
        public string? Country { get; set; }
        public string? CountryCode { get; set; }
        public string? RegionName { get; set; }
        public string? City { get; set; }
        public string? Zip { get; set; }
        public string? Timezone { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public string? Message { get; set; }
    }
}
