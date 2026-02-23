using System.Net;
using System.Net.Http;

namespace Mamey.Http;

public class ApiResponse
{
    public HttpResponseMessage HttpResponse { get; }
    public bool Succeeded { get; }
    public ErrorResponse Error { get; }
    public HttpStatusCode StatusCode { get; }

    public ApiResponse(HttpResponseMessage httpResponse, bool succeeded, HttpStatusCode statusCode, ErrorResponse error = null)
    {
        HttpResponse = httpResponse;
        Succeeded = succeeded;
        Error = error;
        StatusCode = statusCode;
    }

    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Reason { get; set; }
    }
}
public class ApiResponse<T> : ApiResponse
{
    public T? Value { get; }

    public ApiResponse(T? value, HttpResponseMessage httpResponse, bool succeeded, HttpStatusCode statusCode, ErrorResponse error = null)
        : base(httpResponse, succeeded, statusCode, error)
    {
        Value = value;
    }
}
