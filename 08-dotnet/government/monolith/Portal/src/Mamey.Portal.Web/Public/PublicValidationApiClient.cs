using System.Net;
using System.Net.Http.Json;
using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Web.Public;

public interface IPublicValidationApiClient
{
    Task<PublicDocumentValidationResult> ValidateAsync(string documentNumber, CancellationToken ct = default);
    Task<BarcodeScanResult> ScanBarcodeFromImageAsync(Stream imageStream, string fileName, CancellationToken ct = default);
}

public sealed record BarcodeScanResult(
    bool Success,
    string? Message,
    string? DocumentNumber,
    PublicDocumentValidationResult? Validation = null,
    string? BarcodeDataPreview = null);

public sealed class PublicValidationApiClient : IPublicValidationApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PublicValidationApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PublicDocumentValidationResult> ValidateAsync(string documentNumber, CancellationToken ct = default)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
        {
            throw new InvalidOperationException("No active HttpContext.");
        }

        var baseUri = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
        var url = $"{baseUri}/api/public/validate?documentNumber={Uri.EscapeDataString(documentNumber ?? string.Empty)}";

        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url, ct);

        if (response.StatusCode == (HttpStatusCode)429)
        {
            throw new InvalidOperationException("Too many requests. Please wait a minute and try again.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PublicDocumentValidationResult>(cancellationToken: ct);
        return result ?? new PublicDocumentValidationResult(false, false, "Error", null, null, null, null);
    }

    public async Task<BarcodeScanResult> ScanBarcodeFromImageAsync(Stream imageStream, string fileName, CancellationToken ct = default)
    {
        var ctx = _httpContextAccessor.HttpContext;
        if (ctx is null)
        {
            throw new InvalidOperationException("No active HttpContext.");
        }

        var baseUri = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
        var url = $"{baseUri}/api/public/scan-barcode";

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(imageStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        content.Add(streamContent, "image", fileName);

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsync(url, content, ct);

        if (response.StatusCode == (HttpStatusCode)429)
        {
            throw new InvalidOperationException("Too many requests. Please wait a minute and try again.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BarcodeScanResult>(cancellationToken: ct);
        return result ?? new BarcodeScanResult(false, "Error processing image", null);
    }
}




