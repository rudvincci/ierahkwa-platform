using System.Collections.Generic;
using System.Threading.Tasks;
using Chronicle;
using Mamey.MicroMonolith.Abstractions.Time;
using Mamey.MicroMonolith.Infrastructure.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Modules;

namespace Mamey.ApplicationName.Modules.Saga.Api;

internal class SagaModule : IModule
{
    public string Name { get; } = "Saga";
        
    public IEnumerable<string> Policies { get; } = new[]
    {
        "saga"
    };

    public void Register(IServiceCollection services)
    {
        services.AddChronicle();
        services.AddSingleton<IClock, UtcClock>();
    }
        
    public Task Use(IApplicationBuilder app)
    {
        return Task.CompletedTask;
    }
}