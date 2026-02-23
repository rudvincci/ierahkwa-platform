using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Passports.Api
{
    internal class PassportsModule : IModule
    {
        public const string BasePath = "api/passports";        
        public string Name { get; } = "Passports";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"passports"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            // Subscribe to module requests (other modules can call these)
            app.UseModuleRequests()
                // GET passport by ID - callable via _moduleClient.SendAsync<PassportDto>("passports/get", ...)
                .Subscribe<GetPassport, PassportDto?>("passports/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetPassport, PassportDto?>(query, cancellationToken))
                // GET passport by number
                .Subscribe<GetPassportByNumber, PassportDto?>("passports/by-number",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetPassportByNumber, PassportDto?>(query, cancellationToken))
                // GET passports by citizen
                .Subscribe<GetPassportsByCitizen, IEnumerable<PassportSummaryDto>>("passports/by-citizen",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetPassportsByCitizen, IEnumerable<PassportSummaryDto>>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
