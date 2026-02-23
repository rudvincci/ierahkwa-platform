// using Mamey.Http;
//
// namespace Mamey.ApplicationName.BlazorWasm.Clients;
//
// public class FHGApiClient : MameyHttpClient
// {
//     private readonly ILogger<FHGApiClient> _logger;
//
//     public FHGApiClient(ILogger<FHGApiClient> logger, IHttpClientFactory httpClientFactory, HttpClientOptions options,
//         IHttpClientSerializer serializer)
//         : base(httpClientFactory.CreateClient("fhg-api"),
//             options, serializer)
//     {
//         _logger = logger;
//         _logger.LogInformation($"client: {_client.BaseAddress}");
//
//     }
// }