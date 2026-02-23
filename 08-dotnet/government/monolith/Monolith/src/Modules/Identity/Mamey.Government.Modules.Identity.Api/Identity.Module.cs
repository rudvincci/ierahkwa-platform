using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Identity.Api
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
            app.UseModuleRequests()
                .Subscribe<GetUserProfile, UserProfileDto?>("identity/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetUserProfile, UserProfileDto?>(query, cancellationToken))
                .Subscribe<GetUserProfileByEmail, UserProfileDto?>("identity/by-email",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetUserProfileByEmail, UserProfileDto?>(query, cancellationToken))
                .Subscribe<GetUserProfileByAuthenticator, UserProfileDto?>("identity/by-authenticator",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetUserProfileByAuthenticator, UserProfileDto?>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
