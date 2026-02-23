using Mamey.Barcode.Requests;

namespace Mamey.Barcode.Http;

public interface IMameyBarcodeApiClient
{
    Task<BarcodeResponse> GenerateBarcodeAsync<TRequest>(TRequest request) where TRequest : IBarcodeRequest;
}
