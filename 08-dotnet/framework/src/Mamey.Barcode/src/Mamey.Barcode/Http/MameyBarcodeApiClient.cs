using System.Text.Json;
using Mamey.Barcode.Requests;
using Mamey.Http;
using Microsoft.Extensions.Logging;


namespace Mamey.Barcode.Http;

public class MameyBarcodeApiClient : MameyHttpClient, IMameyBarcodeApiClient
{
    private readonly ILogger<MameyBarcodeApiClient> _logger;
    private readonly IMameyBarcodeApiClientResponseHandler _apiResponseHandler;
    private readonly MameyBarcodeApiClient _barcodeApiClient;

    public MameyBarcodeApiClient(ILogger<MameyBarcodeApiClient> logger, IHttpClientFactory clientFactory,
        HttpClientOptions options, IHttpClientSerializer serializer,
        IMameyBarcodeApiClientResponseHandler apiResponseHandler)
        : base(clientFactory.CreateClient("MameyBarcodeApiClient"), options, serializer, logger)
    {
        _logger = logger;
        _logger.LogInformation($"client: {_client.BaseAddress}");
        _apiResponseHandler = apiResponseHandler;
    }

    public async Task<BarcodeResponse> GenerateBarcodeAsync<TRequest>(TRequest request) where TRequest : IBarcodeRequest
    {
        var response = await TryBarcodeRequestAsync(new HttpRequestMessage(HttpMethod.Post, "/generate-barcode")
        {
            Content = GetPayload(request)
        });

        return response;
    }

    private async Task<BarcodeResponse> TryBarcodeRequestAsync(HttpRequestMessage request)
    {
        HttpResponseMessage response = null;
        try
        {
            var requestId = Guid.NewGuid().ToString("N");
            
            _logger.LogInformation($"Sending HTTP request [ID: {requestId}]...");
            response = await _client.SendAsync(request);
            var isValid = response.IsSuccessStatusCode;
            var responseStatus = isValid ? "valid" : "invalid";
            _logger.LogInformation($"Received the {responseStatus} response [ID: {requestId}].");
                var errorPayload = await response.Content.ReadAsStringAsync();
            if (!isValid)
            {
                _logger.LogError(response.ToString());
                _logger.LogError(errorPayload);

                if (!errorPayload.Contains("code"))
                {
                    return new BarcodeResponse(null, response, false, response.StatusCode, new ApiResponse.ErrorResponse
                    {
                        Code = "error",
                        Reason = errorPayload
                    });
                }

                var error = string.IsNullOrWhiteSpace(errorPayload)
                    ? default
                    : JsonSerializer.Deserialize<ApiResponse.ErrorResponse>(errorPayload, JsonExtensions.SerializerOptions);

                return new BarcodeResponse(null, response, false, response.StatusCode, error);
            }
            var bytesPayload = await response.Content.ReadAsByteArrayAsync();
            var result = errorPayload is null
                ? default
                : new BarcodeResponse(bytesPayload, response, true, response.StatusCode);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BarcodeResponse(default, response, false, response.StatusCode);
        }
    }
}
