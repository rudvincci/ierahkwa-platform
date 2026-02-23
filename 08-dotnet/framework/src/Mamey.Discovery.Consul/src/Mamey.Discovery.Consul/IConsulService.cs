using Mamey.Discovery.Consul.Models;

namespace Mamey.Discovery.Consul;

public interface IConsulService
{
    Task<HttpResponseMessage> RegisterServiceAsync(ServiceRegistration registration);
    Task<HttpResponseMessage> DeregisterServiceAsync(string id);
    Task<IDictionary<string, ServiceAgent>> GetServiceAgentsAsync(string service = null);
}