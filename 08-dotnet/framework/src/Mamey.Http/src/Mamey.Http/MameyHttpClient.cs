using System.Net.Http.Headers;
using System.Text;
using Polly;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Mamey.Http;

public class MameyHttpClient : IHttpClient
{
    protected readonly ILogger<MameyHttpClient> _logger;
    protected const string JsonContentType = "application/json";
    protected readonly HttpClient _client;
    protected readonly HttpClientOptions _options;
    protected readonly IHttpClientSerializer _serializer;
    public HttpClient Client => _client;

    public MameyHttpClient(HttpClient client, HttpClientOptions options, IHttpClientSerializer serializer, ILogger<MameyHttpClient> logger)
    {
        _client = client;
        _options = options;
        _serializer = serializer;
        _logger = logger;
    }

    public virtual Task<HttpResponseMessage> GetAsync(string uri)
        => SendAsync(uri, Method.Get);

    public virtual Task<T> GetAsync<T>(string uri, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Get, serializer: serializer);

    public Task<HttpResult<T>> GetResultAsync<T>(string uri, IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Get, serializer: serializer);

    public virtual Task<HttpResponseMessage> PostAsync(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendAsync(uri, Method.Post, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Post, content);

    public virtual Task<T> PostAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Post, GetJsonPayload(data, serializer));

    public Task<T> PostAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Post, content, serializer);

    public Task<HttpResult<T>> PostResultAsync<T>(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Post, GetJsonPayload(data, serializer), serializer);

    public Task<HttpResult<T>> PostResultAsync<T>(string uri, HttpContent content,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Post, content, serializer);

    public virtual Task<HttpResponseMessage> PutAsync(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendAsync(uri, Method.Put, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Put, content);

    public virtual Task<T> PutAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Put, GetJsonPayload(data, serializer), serializer);

    public Task<T> PutAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Put, content, serializer);

    public Task<HttpResult<T>> PutResultAsync<T>(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Put, GetJsonPayload(data, serializer), serializer);

    public Task<HttpResult<T>> PutResultAsync<T>(string uri, HttpContent content,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Put, content, serializer);

    public Task<HttpResponseMessage> PatchAsync(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendAsync(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<HttpResponseMessage> PatchAsync(string uri, HttpContent content)
        => SendAsync(uri, Method.Patch, content);

    public Task<T> PatchAsync<T>(string uri, object data = null, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<T> PatchAsync<T>(string uri, HttpContent content, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Patch, content, serializer);

    public Task<HttpResult<T>> PatchResultAsync<T>(string uri, object data = null,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Patch, GetJsonPayload(data, serializer));

    public Task<HttpResult<T>> PatchResultAsync<T>(string uri, HttpContent content,
        IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Patch, content, serializer);

    public virtual Task<HttpResponseMessage> DeleteAsync(string uri)
        => SendAsync(uri, Method.Delete);



    public Task<T> DeleteAsync<T>(string uri, IHttpClientSerializer serializer = null)
        => SendAsync<T>(uri, Method.Delete, serializer: serializer);
    public Task<HttpResult<T>> DeleteResultAsync<T>(string uri, IHttpClientSerializer serializer = null)
        => SendResultAsync<T>(uri, Method.Delete, serializer: serializer);

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        => Policy.Handle<Exception>()
            .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(Math.Pow(2, r)))
            .ExecuteAsync(() => _client.SendAsync(request));

    public Task<T?> SendAsync<T>(HttpRequestMessage request, IHttpClientSerializer serializer = null)
        => Policy.Handle<Exception>()
            .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(Math.Pow(2, r)))
            .ExecuteAsync(async () =>
            {
                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                var stream = await response.Content.ReadAsStreamAsync();

                return await DeserializeJsonFromStream<T>(stream, serializer);
            });

    public Task<HttpResult<T>> SendResultAsync<T>(HttpRequestMessage request,
        IHttpClientSerializer serializer = null)
        => Policy.Handle<Exception>()
            .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(Math.Pow(2, r)))
            .ExecuteAsync(async () =>
            {
                var response = await _client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResult<T>(default, response);
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var result = await DeserializeJsonFromStream<T>(stream, serializer);

                return new HttpResult<T>(result, response);
            });

    public void SetHeaders(IDictionary<string, string> headers)
    {
        if (headers is null)
        {
            return;
        }

        foreach (var (key, value) in headers)
        {
            if (string.IsNullOrEmpty(key))
            {
                continue;
            }

            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }
    }

    public void SetHeaders(Action<HttpRequestHeaders> headers) => headers?.Invoke(_client.DefaultRequestHeaders);

    protected virtual async Task<T> SendAsync<T>(string uri, Method method, HttpContent content = null,
        IHttpClientSerializer serializer = null)
    {
        var response = await SendAsync(uri, method, content);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }

        var stream = await response.Content.ReadAsStreamAsync();

        return await DeserializeJsonFromStream<T>(stream, serializer);
    }

    protected virtual async Task<HttpResult<T>> SendResultAsync<T>(string uri, Method method,
        HttpContent content = null, IHttpClientSerializer serializer = null)
    {
        var response = await SendAsync(uri, method, content);
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResult<T>(default, response);
        }

        var stream = await response.Content.ReadAsStreamAsync();
        var result = await DeserializeJsonFromStream<T>(stream, serializer);

        return new HttpResult<T>(result, response);
    }

    protected virtual Task<HttpResponseMessage> SendAsync(string uri, Method method, HttpContent content = null)
        => Policy.Handle<Exception>()
            .WaitAndRetryAsync(_options.Retries, r => TimeSpan.FromSeconds(Math.Pow(2, r)))
            .ExecuteAsync(() =>
            {
                var requestUri = uri.StartsWith("http") ? uri : $"http://{uri}";

                return GetResponseAsync(requestUri, method, content);
            });

    protected virtual Task<HttpResponseMessage> GetResponseAsync(string uri, Method method,
        HttpContent content = null)
        => method switch
        {
            Method.Get => _client.GetAsync(uri),
            Method.Post => _client.PostAsync(uri, content),
            Method.Put => _client.PutAsync(uri, content),
            Method.Patch => _client.PatchAsync(uri, content),
            Method.Delete => _client.DeleteAsync(uri),
            _ => throw new InvalidOperationException($"Unsupported HTTP method: {method}")
        };

    protected StringContent GetJsonPayload(object data, IHttpClientSerializer serializer = null)
    {
        if (data is null)
        {
            return null;
        }

        serializer ??= _serializer;
        var content = new StringContent(serializer.Serialize(data), Encoding.UTF8, JsonContentType);
        if (_options.RemoveCharsetFromContentType && content.Headers.ContentType is not null)
        {
            content.Headers.ContentType.CharSet = null;
        }

        return content;
    }

    protected async Task<T> DeserializeJsonFromStream<T>(Stream stream, IHttpClientSerializer serializer = null)
    {
        if (stream is null || stream.CanRead is false)
        {
            return default;
        }

        serializer ??= _serializer;
        return await serializer.DeserializeAsync<T>(stream);
    }

    protected enum Method
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }

    public virtual Task<ApiResponse<T>> GetApiResponseAsync<T>(string endpoint)
    => TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Get, endpoint));

    public virtual async Task<ApiResponse> PostApiResponseAsync(string endpoint, object request)
    {
        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }
    public virtual Task<ApiResponse> PostApiResponseAsync(string endpoint, object request, IEnumerable<KeyValuePair<string, string>>? headers = null)
    {
        throw new NotImplementedException();
    }
    public virtual async Task<ApiResponse<T>> PostApiResponseAsync<T>(string endpoint, object request)
    {
        //_logger.LogError(endpoint);
        var response = await TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }

    public virtual async Task<ApiResponse<U>> PostApiResponseAsync<T, U>(string endpoint, T request, IEnumerable<KeyValuePair<string, string>>? headers = null)
    {
        if (headers is not null)
        {
            foreach (var header in headers)
            {
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
        var payload = GetPayload(request);
        var response = await TryRequestAsync<U>(new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }

    public virtual async Task<ApiResponse> PutApiResponseAsync(string endpoint, object request)
    {
        var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }
    public virtual async Task<ApiResponse> PutApiResponseAsync<T>(string endpoint, T request)
    {
        var response = await TryRequestAsync<T>(new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = GetPayload(request)
        });

        return response;
    }
    public virtual  Task<ApiResponse> DeleteApiResponseAsync(string endpoint)
    {
        throw new NotImplementedException();
        //var response = await TryRequestAsync<object>(new HttpRequestMessage(HttpMethod.Delete, endpoint)
        //{
        //    Content = GetPayload(request)
        //});

        //return response;
    }
 
    protected static StringContent GetPayload<T>(T request)
        => new(JsonSerializer.Serialize(request, JsonExtensions.SerializerOptions), Encoding.UTF8, "application/json");


    protected async Task<ApiResponse<T>> TryRequestAsync<T>(HttpRequestMessage request)
    {
        HttpResponseMessage response = null;
        try
        {
            var requestId = Guid.NewGuid().ToString("N");
            //var accessToken = await GetOnBehalfTokenAsync();
            //if (!string.IsNullOrEmpty(accessToken))
            //{
            //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //}

            //_logger.LogInformation($"Sending HTTP request [ID: {requestId}]...");
            response = await _client.SendAsync(request);
            var isValid = response.IsSuccessStatusCode;
            var responseStatus = isValid ? "valid" : "invalid";
            //_logger.LogInformation($"Received the {responseStatus} response [ID: {requestId}].");
            var payload = await response.Content.ReadAsStringAsync();
            if (!isValid)
            {
                //_logger.LogError(response.ToString());
                //_logger.LogError(payload);

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
                    : JsonSerializer.Deserialize<ApiResponse.ErrorResponse>(payload, JsonExtensions.SerializerOptions);

                return new ApiResponse<T>(default, response, false, response.StatusCode, error);
            }

            var result = string.IsNullOrWhiteSpace(payload)
                ? default
                : JsonSerializer.Deserialize<T>(payload, JsonExtensions.SerializerOptions);

            return new ApiResponse<T>(result, response, true, response.StatusCode);
        }
        catch (HttpRequestException requestException)
        {
            Console.WriteLine(string.Empty);
            if (requestException.Message.Contains("The SSL connection could not be established"))
            {

            }
            return new ApiResponse<T>(default, response, false, response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new ApiResponse<T>(default, response, false, response.StatusCode);
            
            
        }
    }

}