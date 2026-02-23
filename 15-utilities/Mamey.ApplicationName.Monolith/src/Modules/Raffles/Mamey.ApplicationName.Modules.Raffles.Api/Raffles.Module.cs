using Mamey.ApplicationName.Modules.Raffles.Infrastructure;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Raffles.Api;

public class RafflesModule : IModule
{
    public const string BasePath = "raffles-module";        
    public string Name { get; } = "Raffles";
    public string Path => BasePath;

    public IEnumerable<string> Policies { get; } = new[] {"raffles"};

    public void Register(IServiceCollection services)
    {
        services.AddRafflesCore();
    }
        
    public async Task Use(IApplicationBuilder app)
    {
        app
            .UseModuleRequests()
            // .Subscribe<CreateBankingProduct, object>("api/bankingproducts", async (dto, sp, cancellationToken) =>
            // {
            //     var service = sp.GetRequiredService<IBankingProductService>();
            //     await service.AddProductAsync(dto.AsEntity());
            //     return null;
            // }
            //    )
            ;
        //await app.UsePostgresDbAsync();
    }
}
