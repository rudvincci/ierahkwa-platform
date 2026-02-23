using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core;
using Mamey.Government.Modules.Tenant.Core.DTO;
using Mamey.Government.Modules.Tenant.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Tenant.Api
{
    internal class TenantModule : IModule
    {
        public const string BasePath = "api/tenants";        
        public string Name { get; } = "Tenant";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"tenants"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            // Subscribe to module requests
            app.UseModuleRequests()
                .Subscribe<GetTenant, TenantDto?>("tenants/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetTenant, TenantDto?>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
