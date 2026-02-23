using Mamey.Ntrada.Configuration;

namespace Mamey.Ntrada
{
    internal interface IUpstreamBuilder
    {
        string Build(Module module, Route route);
    }
}