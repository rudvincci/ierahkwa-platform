using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mamey.Authentik.Exceptions;

namespace Mamey.Authentik.Handlers;

/// <summary>
/// HTTP message handler that maps HTTP errors to Authentik exceptions.
/// </summary>
public class AuthentikErrorHandler : DelegatingHandler
{
    private readonly ILogger<AuthentikErrorHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikErrorHandler"/> class.
    /// </summary>
    public AuthentikErrorHandler(ILogger<AuthentikErrorHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await HandleErrorResponseAsync(request, response, cancellationToken);
        }

        return response;
    }

    private async Task HandleErrorResponseAsync(
        HttpRequestMessage request,
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
        var requestUri = request.RequestUri?.ToString();
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        _logger.LogWarning(
            "HTTP error {StatusCode} for {Method} {Uri}: {Body}",
            statusCode,
            request.Method,
            requestUri,
            responseBody);

        AuthentikApiException exception = statusCode switch
        {
            400 => CreateValidationException(responseBody, requestUri),
            401 => new AuthentikAuthenticationException(
                "Authentication failed. Please check your credentials.",
                responseBody,
                requestUri),
            403 => new AuthentikAuthorizationException(
                "Access forbidden. You don't have permission to perform this action.",
                responseBody,
                requestUri),
            404 => new AuthentikNotFoundException(
                "Resource not found.",
                null,
                responseBody,
                requestUri),
            429 => CreateRateLimitException(responseBody, requestUri, response),
            >= 500 => new AuthentikApiException(
                statusCode,
                $"Server error: {response.ReasonPhrase ?? "Unknown error"}",
                responseBody,
                requestUri),
            _ => new AuthentikApiException(
                statusCode,
                $"HTTP error {statusCode}: {response.ReasonPhrase ?? "Unknown error"}",
                responseBody,
                requestUri)
        };

        throw exception;
    }

    private AuthentikValidationException CreateValidationException(string? responseBody, string? requestUri)
    {
        Dictionary<string, string[]>? validationErrors = null;

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                var json = JsonDocument.Parse(responseBody);
                if (json.RootElement.TryGetProperty("errors", out var errorsElement))
                {
                    validationErrors = new Dictionary<string, string[]>();
                    foreach (var error in errorsElement.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            validationErrors[error.Name] = error.Value.EnumerateArray()
                                .Select(e => e.GetString() ?? string.Empty)
                                .ToArray();
                        }
                        else
                        {
                            validationErrors[error.Name] = new[] { error.Value.GetString() ?? string.Empty };
                        }
                    }
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors
            }
        }

        return new AuthentikValidationException(
            "Request validation failed. Please check your input.",
            validationErrors,
            responseBody,
            requestUri);
    }

    private AuthentikRateLimitException CreateRateLimitException(
        string? responseBody,
        string? requestUri,
        HttpResponseMessage response)
    {
        int? retryAfterSeconds = null;

        if (response.Headers.RetryAfter?.Delta.HasValue == true)
        {
            retryAfterSeconds = (int)response.Headers.RetryAfter.Delta.Value.TotalSeconds;
        }
        else if (response.Headers.RetryAfter?.Date.HasValue == true)
        {
            var delay = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            if (delay.TotalSeconds > 0)
            {
                retryAfterSeconds = (int)delay.TotalSeconds;
            }
        }

        return new AuthentikRateLimitException(
            "Rate limit exceeded. Please retry after the specified time.",
            retryAfterSeconds,
            responseBody,
            requestUri);
    }
}
