using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.TravelIdentities.Api
{
    internal class TravelIdentitiesModule : IModule
    {
        public const string BasePath = "api/travel-identities";        
        public string Name { get; } = "TravelIdentities";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"travel-identities"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            // Subscribe to module requests
            app.UseModuleRequests()
                .Subscribe<GetTravelIdentity, TravelIdentityDto?>("travel-identities/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetTravelIdentity, TravelIdentityDto?>(query, cancellationToken))
                .Subscribe<GetTravelIdentityByNumber, TravelIdentityDto?>("travel-identities/by-number",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetTravelIdentityByNumber, TravelIdentityDto?>(query, cancellationToken))
                .Subscribe<GetTravelIdentitiesByCitizen, IEnumerable<TravelIdentitySummaryDto>>("travel-identities/by-citizen",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetTravelIdentitiesByCitizen, IEnumerable<TravelIdentitySummaryDto>>(query, cancellationToken));
            // app.UseContracts()
            //     .Register<SignedUpContract>()
            //     .Register<UserStateUpdatedContract>();
            await app.UseCoreAsync();
        }
    }
}
