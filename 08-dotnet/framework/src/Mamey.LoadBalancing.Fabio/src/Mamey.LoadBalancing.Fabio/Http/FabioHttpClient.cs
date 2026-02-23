using Mamey.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.LoadBalancing.Fabio.Http;

internal sealed class FabioHttpClient : MameyMicroserviceHttpClient, IFabioHttpClient
{
    public FabioHttpClient(HttpClient client, HttpClientOptions options, IHttpClientSerializer serializer,
        ICorrelationContextFactory correlationContextFactory, ICorrelationIdFactory correlationIdFactory, ILogger<FabioHttpClient> logger)
        : base(client, options, serializer, correlationContextFactory, correlationIdFactory, logger)
    {
    }
}