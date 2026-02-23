using Mamey.Persistence.OpenStack.OCS.Http;

namespace Mamey.Persistence.OpenStack.OCS.RequestHandler;

internal interface IRequestHandler
{
    Task<HttpRequestResult> Send(Func<IHttpRequestBuilder, IHttpRequestBuilder> httpRequestBuilder);
}