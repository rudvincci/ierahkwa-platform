using Microsoft.Extensions.Logging;

namespace Mamey.Http;

public class MameyMicroserviceHttpClient : MameyHttpClient
{
    protected const string JsonContentType = "application/json";
    protected readonly HttpClient _client;
    protected readonly HttpClientOptions _options;
    protected readonly IHttpClientSerializer _serializer;
    private readonly ILogger<MameyMicroserviceHttpClient> _logger;
    public HttpClient Client => _client;

    public MameyMicroserviceHttpClient(HttpClient client, HttpClientOptions options, IHttpClientSerializer serializer,
        ICorrelationContextFactory correlationContextFactory, ICorrelationIdFactory correlationIdFactory, ILogger<MameyMicroserviceHttpClient> logger)
        : base(client, options, serializer, logger)
    {
        _client = client;
        _options = options;
        _serializer = serializer;
        _logger = logger;
        if (!string.IsNullOrWhiteSpace(_options.CorrelationContextHeader))
        {
            var correlationContext = correlationContextFactory.Create();
            _client.DefaultRequestHeaders.TryAddWithoutValidation(_options.CorrelationContextHeader,
                correlationContext);
        }

        if (!string.IsNullOrWhiteSpace(_options.CorrelationIdHeader))
        {
            var correlationId = correlationIdFactory.Create();
            _client.DefaultRequestHeaders.TryAddWithoutValidation(_options.CorrelationIdHeader,
                correlationId);
        }
    }
}
