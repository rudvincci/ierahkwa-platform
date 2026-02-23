using System.Collections.Generic;
using System.Threading.Tasks;
using Chronicle;
using Mamey.Government.Modules.Saga.Api.Integration;
using Mamey.MicroMonolith.Abstractions.Time;
using Mamey.MicroMonolith.Infrastructure.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Modules;

namespace Mamey.Government.Modules.Saga.Api;

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
        services.AddSagaIntegration();
    }
        
    public Task Use(IApplicationBuilder app)
    {
        return Task.CompletedTask;
    }
}
