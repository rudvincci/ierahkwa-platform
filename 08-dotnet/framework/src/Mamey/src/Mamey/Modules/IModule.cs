using Microsoft.AspNetCore.Builder;

namespace Mamey.Modules;

public interface IModule
{
    string Name { get; }
    IEnumerable<string> Policies => null;
    void Register(IServiceCollection services);
    Task Use(IApplicationBuilder app);
}