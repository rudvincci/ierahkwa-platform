using System.Net.Http.Json;
using System.Text.Json;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.UI.Services;

/// <summary>
/// Base class for API services providing common HTTP operations.
/// </summary>
public abstract class ApiServiceBase
{
    protected readonly HttpClient HttpClient;
    protected readonly ILogger Logger;
    protected readonly Guid? _tenantId;

    
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    protected ApiServiceBase(HttpClient httpClient, ILogger logger, AppOptions appOptions)
    {
        HttpClient = httpClient;
        Logger = logger;
        Guid tenantId;
        if (Guid.TryParse(appOptions.TenantId, out tenantId))
        {
            _tenantId = tenantId;
            Logger.LogInformation("Tenant ID loaded from AppOptions: {TenantId}", _tenantId);
        }
        else
        {
            _tenantId = null;
            Logger.LogWarning("Failed to parse TenantId from AppOptions. Value: '{TenantId}'. X-TENANT-ID header will not be added.", appOptions.TenantId ?? "null");
        }
    }

    /// <summary>
    /// Adds the X-TENANT-ID header to the request if tenantId is provided.
    /// </summary>
    protected HttpRequestMessage CreateRequest(HttpMethod method, string url, Guid? tenantId = null)
    {
        var request = new HttpRequestMessage(method, url);
        // Use the parameter if provided, otherwise fall back to the instance field
        var effectiveTenantId = tenantId ?? _tenantId;
        if (effectiveTenantId.HasValue)
        {
            request.Headers.Add("X-TENANT-ID", effectiveTenantId.Value.ToString());
            Logger.LogDebug("Added X-TENANT-ID header: {TenantId} for {Method} {Url}", effectiveTenantId.Value, method, url);
        }
        else
        {
            Logger.LogDebug("No tenant ID available for {Method} request to {Url}", method, url);
        }
        return request;
    }

    protected async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        return await GetAsync<T>(url, null, cancellationToken);
    }

    protected async Task<T?> GetAsync<T>(string url, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("GET {Url}", url);
            var request = CreateRequest(HttpMethod.Get, url, tenantId);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                Logger.LogWarning("GET {Url} returned {StatusCode}: {Content}", url, response.StatusCode, preview);
                return default;
            }
            
            // Check if response is actually JSON
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType != null && !contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var preview = content.Length > 500 ? content.Substring(0, 500) + "..." : content;
                Logger.LogWarning("GET {Url} returned non-JSON content ({ContentType}): {Content}", url, contentType, preview);
                return default;
            }
            
            // Read content as string first, then parse - this allows us to log it if parsing fails
            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return default;
            }
            
            try
            {
                return JsonSerializer.Deserialize<T>(jsonContent, JsonOptions);
            }
            catch (JsonException jsonEx)
            {
                var preview = jsonContent.Length > 500 ? jsonContent.Substring(0, 500) + "..." : jsonContent;
                Logger.LogError(jsonEx, "Error parsing JSON from GET {Url}. Content: {Content}", url, preview);
                return default;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling GET {Url}", url);
            return default;
        }
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<TRequest, TResponse>(url, data, null, cancellationToken);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url, 
        TRequest data, 
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("POST {Url}", url);
            var request = CreateRequest(HttpMethod.Post, url, tenantId);
            request.Content = JsonContent.Create(data, options: JsonOptions);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogWarning("POST {Url} returned {StatusCode}: {Error}", url, response.StatusCode, error);
                return default;
            }
            
            if (response.Content.Headers.ContentLength == 0)
            {
                return default;
            }
            
            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling POST {Url}", url);
            throw;
        }
    }

    protected async Task<bool> PostAsync<TRequest>(
        string url, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        return await PostAsync(url, data, null, cancellationToken);
    }

    protected async Task<bool> PostAsync<TRequest>(
        string url, 
        TRequest data, 
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("POST {Url}", url);
            var request = CreateRequest(HttpMethod.Post, url, tenantId);
            request.Content = JsonContent.Create(data, options: JsonOptions);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogWarning("POST {Url} returned {StatusCode}: {Error}", url, response.StatusCode, error);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling POST {Url}", url);
            throw;
        }
    }

    protected async Task<bool> PostAsync(string url, CancellationToken cancellationToken = default)
    {
        return await PostAsync(url, null, cancellationToken);
    }

    protected async Task<bool> PostAsync(string url, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("POST {Url}", url);
            var request = CreateRequest(HttpMethod.Post, url, tenantId);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogWarning("POST {Url} returned {StatusCode}: {Error}", url, response.StatusCode, error);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling POST {Url}", url);
            throw;
        }
    }

    protected async Task<bool> PutAsync<TRequest>(
        string url, 
        TRequest data, 
        CancellationToken cancellationToken = default)
    {
        return await PutAsync(url, data, null, cancellationToken);
    }

    protected async Task<bool> PutAsync<TRequest>(
        string url, 
        TRequest data, 
        Guid? tenantId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("PUT {Url}", url);
            var request = CreateRequest(HttpMethod.Put, url, tenantId);
            request.Content = JsonContent.Create(data, options: JsonOptions);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                Logger.LogWarning("PUT {Url} returned {StatusCode}: {Error}", url, response.StatusCode, error);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling PUT {Url}", url);
            throw;
        }
    }

    protected async Task<bool> DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync(url, null, cancellationToken);
    }

    protected async Task<bool> DeleteAsync(string url, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogDebug("DELETE {Url}", url);
            var request = CreateRequest(HttpMethod.Delete, url, tenantId);
            var response = await HttpClient.SendAsync(request, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogWarning("DELETE {Url} returned {StatusCode}", url, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calling DELETE {Url}", url);
            throw;
        }
    }

    protected string BuildQueryString(params (string key, object? value)[] parameters)
    {
        var queryParams = parameters
            .Where(p => p.value != null)
            .Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value!.ToString()!)}");
        
        return string.Join("&", queryParams);
    }
}
