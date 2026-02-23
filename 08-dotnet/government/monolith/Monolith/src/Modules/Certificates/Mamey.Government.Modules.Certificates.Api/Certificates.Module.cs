using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.Government.Modules.Certificates.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Certificates.Api
{
    internal class CertificatesModule : IModule
    {
        public const string BasePath = "api/certificates";        
        public string Name { get; } = "Certificates";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"certificates"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            // Subscribe to module requests
            app.UseModuleRequests()
                .Subscribe<GetCertificate, CertificateDto?>("certificates/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetCertificate, CertificateDto?>(query, cancellationToken))
                .Subscribe<GetCertificateByNumber, CertificateDto?>("certificates/by-number",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetCertificateByNumber, CertificateDto?>(query, cancellationToken))
                .Subscribe<GetCertificatesByCitizen, IEnumerable<CertificateSummaryDto>>("certificates/by-citizen",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetCertificatesByCitizen, IEnumerable<CertificateSummaryDto>>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
