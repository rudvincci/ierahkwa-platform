using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Identity.Core;
using Mamey.ApplicationName.Modules.Identity.Core.DTO;
using Mamey.ApplicationName.Modules.Identity.Core.Queries;
using Mamey.CQRS.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Bank.BlazorBootstrapper")]
namespace Mamey.ApplicationName.Modules.Identity.Api
{
    internal class IdentityModule : IModule
    {
        public const string BasePath = "api/identity";        
        public string Name { get; } = "Identity";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"identity"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            app
                .UseModuleRequests()
                // .Subscribe<ApplicationUserDto, object>("identity/create", async (dto, sp, cancellationToken) =>
                // {
                //     var service = sp.GetRequiredService<IIdentityService>();
                //     await service.CreateAsync(dto);
                //     return null;
                // })
                .Subscribe<GetUserById, ApplicationUserDto?>($"{BasePath}/account/", 
                    async (query, serviceProvider, cancellationToken) =>
                    {
                        var dispatcher = serviceProvider.GetRequiredService<IQueryDispatcher>();
                        var result = await dispatcher.QueryAsync<GetUserById, ApplicationUserDto?>(query, cancellationToken);
                        return result;
                    });
            // app.UseContracts()
            //     .Register<SignedUpContract>()
            //     .Register<UserStateUpdatedContract>();
            await app.UseCoreAsync();
        }
    }
}