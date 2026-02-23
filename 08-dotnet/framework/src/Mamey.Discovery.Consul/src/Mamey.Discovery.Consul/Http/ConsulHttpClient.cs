using Mamey.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Discovery.Consul.Http;

internal sealed class ConsulHttpClient : MameyMicroserviceHttpClient, IConsulHttpClient
{
    public ConsulHttpClient(HttpClient client, HttpClientOptions options, IHttpClientSerializer serializer,
        ICorrelationContextFactory correlationContextFactory, ICorrelationIdFactory correlationIdFactory, ILogger<ConsulHttpClient> logger)
        : base(client, options, serializer, correlationContextFactory, correlationIdFactory, logger)
    {
    }
}