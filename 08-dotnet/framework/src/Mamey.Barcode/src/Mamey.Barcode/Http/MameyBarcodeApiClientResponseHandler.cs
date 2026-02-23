using System.Net;
using Mamey.Http;

namespace Mamey.Barcode.Http;

public class MameyBarcodeApiClientResponseHandler : IMameyBarcodeApiClientResponseHandler
{
    public async Task<ApiResponse> HandleAsync(Task<ApiResponse> request)
    {
        var response = await request;
        if (response.Succeeded)
        {
            return response;
        }

        await HandleErrorAsync(response);
        return default;
    }

    public async Task<BarcodeResponse> HandleAsync(Task<ApiResponse<byte[]>> request)
    {
        var response = (await request);
        
        if (response.Succeeded)
        {
            return new BarcodeResponse(response.Value, response.HttpResponse, response.Succeeded, response.StatusCode, null);
        }

        await HandleErrorAsync(response);
        return default;
    }

    private async Task HandleErrorAsync(ApiResponse response)
        => await Task.FromResult(() =>
        {
            if (response?.HttpResponse is null)
            {
                return;
            }

            if (response.HttpResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            if (response.Error is { })
            {
            }

        });
}
public interface IMameyBarcodeApiClientResponseHandler
{
    Task<ApiResponse> HandleAsync(Task<ApiResponse> request);
    Task<BarcodeResponse> HandleAsync(Task<ApiResponse<byte[]>> request);
}

