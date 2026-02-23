using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core;
using Mamey.ApplicationName.Modules.Products.Core.Commands;
using Mamey.ApplicationName.Modules.Products.Core.Mappings;
using Mamey.ApplicationName.Modules.Products.Core.Services;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Products.Api
{
    internal class ProductsModule : IModule
    {
        public const string BasePath = "products-module";        
        public string Name { get; } = "Products";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"products"};

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }
        
        public async Task Use(IApplicationBuilder app)
        {
            app
                .UseModuleRequests()
                .Subscribe<CreateBankingProduct, object>("api/bankingproducts", async (dto, sp, cancellationToken) =>
                {
                    var service = sp.GetRequiredService<IBankingProductService>();
                    await service.AddProductAsync(dto.AsEntity());
                    return null;
                });
            //await app.UsePostgresDbAsync();
        }
    }
}