using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Authentik.Handlers;

/// <summary>
/// HTTP message handler that logs requests and responses.
/// </summary>
public class AuthentikLoggingHandler : DelegatingHandler
{
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikLoggingHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikLoggingHandler"/> class.
    /// </summary>
    public AuthentikLoggingHandler(
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikLoggingHandler> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        if (_logger.IsEnabled(_options.LogLevel))
        {
            await LogRequestAsync(request, requestId);
        }

        HttpResponseMessage? response = null;
        Exception? exception = null;

        try
        {
            response = await base.SendAsync(request, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            if (_logger.IsEnabled(_options.LogLevel))
            {
                if (response != null)
                {
                    await LogResponseAsync(response, requestId, stopwatch.Elapsed);
                }
                else if (exception != null)
                {
                    LogException(exception, requestId, stopwatch.Elapsed);
                }
            }
        }
    }

    private async Task LogRequestAsync(HttpRequestMessage request, string requestId)
    {
        var method = request.Method.Method;
        var uri = request.RequestUri?.ToString() ?? "unknown";
        var headers = MaskSensitiveHeaders(request.Headers.ToString());

        _logger.Log(
            _options.LogLevel,
            "[{RequestId}] {Method} {Uri}\nHeaders: {Headers}",
            requestId,
            method,
            uri,
            headers);

        if (request.Content != null && _options.LogLevel <= LogLevel.Debug)
        {
            var body = await MaskSensitiveContent(await request.Content.ReadAsStringAsync());
            _logger.LogDebug("[{RequestId}] Request Body: {Body}", requestId, body);
        }
    }

    private async Task LogResponseAsync(HttpResponseMessage response, string requestId, TimeSpan elapsed)
    {
        var statusCode = (int)response.StatusCode;
        var reasonPhrase = response.ReasonPhrase ?? "Unknown";
        var headers = MaskSensitiveHeaders(response.Headers.ToString());

        _logger.Log(
            _options.LogLevel,
            "[{RequestId}] {StatusCode} {ReasonPhrase} ({ElapsedMs}ms)\nHeaders: {Headers}",
            requestId,
            statusCode,
            reasonPhrase,
            elapsed.TotalMilliseconds,
            headers);

        if (response.Content != null && _options.LogLevel <= LogLevel.Debug)
        {
            var body = await MaskSensitiveContent(await response.Content.ReadAsStringAsync());
            _logger.LogDebug("[{RequestId}] Response Body: {Body}", requestId, body);
        }
    }

    private void LogException(Exception exception, string requestId, TimeSpan elapsed)
    {
        _logger.LogError(
            exception,
            "[{RequestId}] Request failed after {ElapsedMs}ms: {Message}",
            requestId,
            elapsed.TotalMilliseconds,
            exception.Message);
    }

    private string MaskSensitiveHeaders(string headers)
    {
        // Mask Authorization header
        return headers
            .Replace("Authorization: Bearer ", "Authorization: Bearer ***", StringComparison.OrdinalIgnoreCase)
            .Replace("Authorization: Basic ", "Authorization: Basic ***", StringComparison.OrdinalIgnoreCase);
    }

    private Task<string> MaskSensitiveContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Task.FromResult(content);
        }

        // Try to mask common sensitive fields in JSON
        try
        {
            var masked = content
                .Replace("\"password\":\"", "\"password\":\"***", StringComparison.OrdinalIgnoreCase)
                .Replace("\"token\":\"", "\"token\":\"***", StringComparison.OrdinalIgnoreCase)
                .Replace("\"access_token\":\"", "\"access_token\":\"***", StringComparison.OrdinalIgnoreCase)
                .Replace("\"refresh_token\":\"", "\"refresh_token\":\"***", StringComparison.OrdinalIgnoreCase)
                .Replace("\"client_secret\":\"", "\"client_secret\":\"***", StringComparison.OrdinalIgnoreCase);

            return Task.FromResult(masked);
        }
        catch
        {
            return Task.FromResult(content);
        }
    }
}
