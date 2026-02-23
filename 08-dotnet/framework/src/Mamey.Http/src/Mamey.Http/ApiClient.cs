//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;
//using Mamey;
//using Microsoft.Extensions.Logging;

//namespace Mamey.Http;

//public class ApiClient : IHttpClient
//{
//    private readonly ILogger<ApiClient> _logger;
//    protected const string JsonContentType = "application/json";
//    protected readonly HttpClient _client;
//    protected readonly HttpClientOptions _options;
//    protected readonly IHttpClientSerializer _serializer;

//    public HttpClient Client => _client;

//    public ApiClient(ILogger<ApiClient> logger, IHttpClientFactory clientFactory, string clientName = "MameyApiClient")
        
//    {
//        _logger = logger;
        
//        _client = clientFactory.CreateClient(clientName);
//    }

//    public Task<ApiResponse<T>> GetApiResponseAsync<T>(string endpoint)
//    => TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Get, endpoint));

//    public async Task<ApiResponse> PostApiResponseAsync(string endpoint, object request)
//    {
//        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Post, endpoint)
//        {
//            Content = GetPayload(request)
//        });

//        return response;
//    }
//    public Task<ApiResponse> PostApiResponseAsync(string endpoint, object request, IEnumerable<KeyValuePair<string, string>>? headers = null)
//    {
//        throw new NotImplementedException();
//    }
//    public async Task<ApiResponse<T>> PostApiResponseAsync<T>(string endpoint, object request)
//    {
//        _logger.LogError(endpoint);
//        var response = await TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Post, endpoint)
//        {
//            Content = GetPayload(request)
//        });

//        return response;
//    }

//    public async Task<ApiResponse<U>> PostApiResponseAsync<T, U>(string endpoint, T request, IEnumerable<KeyValuePair<string, string>>? headers = null)
//    {
//        if (headers is not null)
//        {
//            foreach (var header in headers)
//            {
//                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
//            }
//        }
//        var payload = GetPayload(request);
//        var response = await TryRequestAsync<U>(new HttpRequestMessage(HttpMethod.Post, endpoint)
//        {
//            Content = GetPayload(request)
//        });

//        return response;
//    }

//    public async Task<ApiResponse> PutApiResponseAsync(string endpoint, object request)
//    {
//        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Put, endpoint)
//        {
//            Content = GetPayload(request)
//        });

//        return response;
//    }
//    public async Task<ApiResponse> PutApiResponseAsync<T>(string endpoint, T request)
//    {
//        var response = await TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Put, endpoint)
//        {
//            Content = GetPayload(request)
//        });

//        return response;
//    }

//    //public async Task<string?> GetOnBehalfTokenAsync()
//    //{
//    //    try
//    //    {
//    //        if (_httpContextAccessor is not null && _httpContextAccessor.HttpContext is not null)
//    //        {                
//    //            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("accessToken");
//    //            return token;
//    //        }


//    //        //string incomingToken = authenticateInfo.Properties.Items["access_token"];
//    //        //return incomingToken;
//    //        return null;
//    //    }
//    //    catch (Exception ex)
//    //    {
//    //        throw;
//    //    }

//    //}

//    public async Task<ApiResponse> DeleteAsync(string endpoint)
//    {
//        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Delete, endpoint));

//        return response;
//    }

//    protected static StringContent GetPayload<T>(T request)
//        => new(JsonSerializer.Serialize(request, JsonExtensions.SerializerOptions), Encoding.UTF8, "application/json");

//    protected async Task<ApiResponse<T>> TryRequestAsync<T>(HttpRequestMessage request)
//    {
//        HttpResponseMessage response = null;
//        try
//        {
//            var requestId = Guid.NewGuid().ToString("N");
//            //var accessToken = await GetOnBehalfTokenAsync();
//            //if (!string.IsNullOrEmpty(accessToken))
//            //{
//            //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//            //}

//            _logger.LogInformation($"Sending HTTP request [ID: {requestId}]...");
//            response = await _client.SendAsync(request);
//            var isValid = response.IsSuccessStatusCode;
//            var responseStatus = isValid ? "valid" : "invalid";
//            _logger.LogInformation($"Received the {responseStatus} response [ID: {requestId}].");
//            var payload = await response.Content.ReadAsStringAsync();
//            if (!isValid)
//            {
//                _logger.LogError(response.ToString());
//                _logger.LogError(payload);

//                if (!payload.Contains("code"))
//                {
//                    return new ApiResponse<T>(default, response, false, response.StatusCode, new ApiResponse.ErrorResponse
//                    {
//                        Code = "error",
//                        Reason = payload
//                    });
//                }

//                var error = string.IsNullOrWhiteSpace(payload)
//                    ? default
//                    : JsonSerializer.Deserialize<ApiResponse.ErrorResponse>(payload, JsonExtensions.SerializerOptions);

//                return new ApiResponse<T>(default, response, false, response.StatusCode, error);
//            }

//            var result = string.IsNullOrWhiteSpace(payload)
//                ? default
//                : JsonSerializer.Deserialize<T>(payload, JsonExtensions.SerializerOptions);

//            return new ApiResponse<T>(result, response, true, response.StatusCode);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, ex.Message);
//            return new ApiResponse<T>(default, response, false, response.StatusCode);
//        }
//    }

//    public Task<HttpResponseMessage> GetAsync(string uri)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> GetAsync<T>(string uri, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> GetResultAsync<T>(string uri, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PostAsync(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PostAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PostAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PostResultAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PostResultAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PutAsync(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PutAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PutAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PutResultAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PutResultAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PatchAsync(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PatchAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> PatchAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PatchResultAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> PatchResultAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    Task<HttpResponseMessage> IHttpClient.DeleteAsync(string uri)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> DeleteAsync<T>(string uri, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> DeleteResultAsync<T>(string uri, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<T> SendAsync<T>(HttpRequestMessage request, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<HttpResult<T>> SendResultAsync<T>(HttpRequestMessage request, IHttpClientSerializer serializer = null)
//    {
//        throw new NotImplementedException();
//    }

//    public void SetHeaders(IDictionary<string, string> headers)
//    {
//        throw new NotImplementedException();
//    }

//    public void SetHeaders(Action<HttpRequestHeaders> headers)
//    {
//        throw new NotImplementedException();
//    }
//}