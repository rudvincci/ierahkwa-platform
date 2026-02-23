using Mamey.Discovery.Consul.Models;

namespace Mamey.Discovery.Consul;

public interface IConsulServicesRegistry
{
    Task<ServiceAgent> GetAsync(string name);
}