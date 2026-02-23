using Mamey.Ntrada.Configuration;

namespace Mamey.Ntrada
{
    internal interface IRouteConfigurator
    {
        RouteConfig Configure(Module module, Route route);
    }
}