using System.Net;
using Mamey.Http;

namespace Mamey.Barcode;

public class BarcodeResponse : ApiResponse<byte[]>
{
    public BarcodeResponse(byte[]? bytes, HttpResponseMessage httpResponse, bool succeeded, HttpStatusCode statusCode, ErrorResponse error = null)
        : base(bytes, httpResponse, succeeded, statusCode, error)
    {
        BarcodeBytes = bytes;
        Base64 = Convert.ToBase64String(bytes);
    }
    public BarcodeResponse(ApiResponse<byte[]> response, ErrorResponse errorResponse)
        : base(default, response.HttpResponse, response.Succeeded, response.StatusCode, errorResponse)
    {
        BarcodeBytes = response.Value;
        Base64 = Convert.ToBase64String(response.Value);
    }
    
    public byte[]? BarcodeBytes { get; set; }
    public string? Base64 { get; set; }
}
