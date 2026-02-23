using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mamey.Ntrada
{
    internal interface IDownstreamBuilder
    {
        string GetDownstream(RouteConfig routeConfig, HttpRequest request, RouteData data);
    }
}