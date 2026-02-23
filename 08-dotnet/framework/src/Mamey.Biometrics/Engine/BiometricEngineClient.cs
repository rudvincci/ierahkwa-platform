using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Biometrics.Engine.Models;

namespace Mamey.Biometrics.Engine;

/// <summary>
/// HTTP client for communicating with the Python biometric engine.
/// </summary>
public class BiometricEngineClient : IBiometricEngineClient
{
    private readonly HttpClient _httpClient;
    private readonly BiometricsOptions _options;
    private readonly ILogger<BiometricEngineClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the BiometricEngineClient.
    /// </summary>
    public BiometricEngineClient(
        HttpClient httpClient,
        IOptions<BiometricsOptions> options,
        ILogger<BiometricEngineClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public async Task<ExtractEncodingResponse> ExtractEncodingAsync(ExtractEncodingRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Extracting face encoding from image");

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/extract-encoding", content, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Extract encoding failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseContent);
                
                return new ExtractEncodingResponse
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}",
                    Message = responseContent
                };
            }

            var result = JsonSerializer.Deserialize<ExtractEncodingResponse>(responseContent, _jsonOptions);
            
            if (result == null)
            {
                _logger.LogError("Failed to deserialize extract encoding response");
                return new ExtractEncodingResponse
                {
                    Success = false,
                    Error = "Deserialization failed",
                    Message = "Failed to parse response from biometric engine"
                };
            }

            _logger.LogDebug("Face encoding extracted successfully");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting face encoding");
            return new ExtractEncodingResponse
            {
                Success = false,
                Error = ex.Message,
                Message = "Failed to extract face encoding"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<CompareEncodingsResponse> CompareEncodingsAsync(CompareEncodingsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Comparing face encodings");

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/compare-encodings", content, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Compare encodings failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseContent);
                
                return new CompareEncodingsResponse
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}",
                    Message = responseContent
                };
            }

            var result = JsonSerializer.Deserialize<CompareEncodingsResponse>(responseContent, _jsonOptions);
            
            if (result == null)
            {
                _logger.LogError("Failed to deserialize compare encodings response");
                return new CompareEncodingsResponse
                {
                    Success = false,
                    Error = "Deserialization failed",
                    Message = "Failed to parse response from biometric engine"
                };
            }

            _logger.LogDebug("Face encodings compared successfully, similarity: {Similarity}", result.Similarity);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error comparing face encodings");
            return new CompareEncodingsResponse
            {
                Success = false,
                Error = ex.Message,
                Message = "Failed to compare face encodings"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<DetectFaceResponse> DetectFaceAsync(DetectFaceRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Detecting faces in image");

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/detect-face", content, cancellationToken);
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Detect face failed with status {StatusCode}: {Response}", 
                    response.StatusCode, responseContent);
                
                return new DetectFaceResponse
                {
                    Success = false,
                    Error = $"HTTP {response.StatusCode}",
                    Message = responseContent
                };
            }

            var result = JsonSerializer.Deserialize<DetectFaceResponse>(responseContent, _jsonOptions);
            
            if (result == null)
            {
                _logger.LogError("Failed to deserialize detect face response");
                return new DetectFaceResponse
                {
                    Success = false,
                    Error = "Deserialization failed",
                    Message = "Failed to parse response from biometric engine"
                };
            }

            _logger.LogDebug("Face detection completed, found {FacesDetected} faces", result.FacesDetected);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting faces");
            return new DetectFaceResponse
            {
                Success = false,
                Error = ex.Message,
                Message = "Failed to detect faces"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking biometric engine health");

            var response = await _httpClient.GetAsync("/api/health", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Health check failed with status {StatusCode}", response.StatusCode);
                return false;
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var healthResponse = JsonSerializer.Deserialize<HealthResponse>(responseContent, _jsonOptions);
            
            var isHealthy = healthResponse?.Status.Equals("healthy", StringComparison.OrdinalIgnoreCase) ?? false;
            
            _logger.LogDebug("Biometric engine health check result: {IsHealthy}", isHealthy);
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Health check failed");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceInfoResponse> GetServiceInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service information");

            var response = await _httpClient.GetAsync("/api/info", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get service info failed with status {StatusCode}", response.StatusCode);
                return new ServiceInfoResponse();
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ServiceInfoResponse>(responseContent, _jsonOptions);
            
            return result ?? new ServiceInfoResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service information");
            return new ServiceInfoResponse();
        }
    }
}

/// <summary>
/// Health response model.
/// </summary>
internal class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public double? UptimeSeconds { get; set; }
    public string Timestamp { get; set; } = string.Empty;
}
