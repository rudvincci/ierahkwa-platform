using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Customers.Core;
using Mamey.ApplicationName.Modules.Customers.Core.DTO;
using Mamey.ApplicationName.Modules.Customers.Core.Events.External;
using Mamey.ApplicationName.Modules.Customers.Core.Queries;
using Mamey.CQRS.Queries;
using Mamey.Modules;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Customers.Api
{
    internal class CustomersModule : IModule
    {
        public const string BasePath = "customers-module";        
        public string Name { get; } = "Customers";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"customers"};

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }
        
        public Task Use(IApplicationBuilder app)
        {
            app.UseModuleRequests()
            .Subscribe<GetCustomer, CustomerDetailsDto?>("customers/get",
                async (query, serviceProvider, cancellationToken)
                    => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                        .QueryAsync<GetCustomer, CustomerDetailsDto?>(query, cancellationToken));

            app.UseContracts()
                .Register<SignedUpContract>()
                .Register<UserStateUpdatedContract>();
            return Task.CompletedTask;
        }
    }
}