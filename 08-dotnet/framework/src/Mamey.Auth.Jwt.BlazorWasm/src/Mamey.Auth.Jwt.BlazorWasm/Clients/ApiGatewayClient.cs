using Mamey.BlazorWasm.Http;
using Mamey.BlazorWasm.Types;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mamey.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.Jwt.BlazorWasm.Clients;

public class ApiGatewayClient : MameyHttpClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly ILocalStorageService _localStorageService;
    private readonly ILogger<ApiGatewayClient> _logger;

    public ApiGatewayClient(HttpClient client, HttpClientOptions options, IHttpClientSerializer serializer,
        ILocalStorageService localStorageService, ILogger<ApiGatewayClient> logger)
        : base(client, options, serializer, logger)
    {
        _logger = logger;
    }

    //public ApiGatewayClient(HttpClient client, ILocalStorageService localStorageService,
    //    ILogger<ApiGatewayClient> logger)
    //{
    //    _client = client;
    //    _localStorageService = localStorageService;
    //    _logger = logger;
    //}

    public Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        => TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Get, endpoint));

    public async Task<ApiResponse> PostAsync(string endpoint, object request)
    {
        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object request)
    {
        var response = await TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }

    public async Task<ApiResponse> PutAsync(string endpoint, object request)
    {
        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }

    public async Task<ApiResponse> DeleteAsync(string endpoint)
    {
        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Delete, endpoint));

        return response;
    }

    private static StringContent GetPayload<T>(T request)
        => new(JsonSerializer.Serialize(request, SerializerOptions), Encoding.UTF8, "application/json");

    private async Task<ApiResponse<T>> TryRequestAsync<T>(HttpRequestMessage request)
    {
        HttpResponseMessage response = null;
        try
        {
            var user = await _localStorageService.GetItemAsync<User>("user");
            if (user is { })
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);
            }

            var requestId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"Sending HTTP request [ID: {requestId}]...");
            response = await _client.SendAsync(request);
            var isValid = response.IsSuccessStatusCode;
            var responseStatus = isValid ? "valid" : "invalid";
            _logger.LogInformation($"Received the {responseStatus} response [ID: {requestId}].");
            var payload = await response.Content.ReadAsStringAsync();
            if (!isValid)
            {
                _logger.LogError(response.ToString());
                _logger.LogError(payload);
                
                if (!payload.Contains("code"))
                {
                    return new ApiResponse<T>(default, response, false, response.StatusCode, new ApiResponse.ErrorResponse
                    {
                        Code = "error",
                        Reason = payload
                    });
                }

                var error = string.IsNullOrWhiteSpace(payload)
                    ? default
                    : JsonSerializer.Deserialize<ApiResponse.ErrorResponse>(payload, SerializerOptions);

                return new ApiResponse<T>(default, response, false, response.StatusCode, error);
            }

            var result = string.IsNullOrWhiteSpace(payload)
                ? default
                : JsonSerializer.Deserialize<T>(payload, SerializerOptions);

            return new ApiResponse<T>(result, response, true, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ApiResponse<T>(default, response, false, response.StatusCode);
        }
    }
}
