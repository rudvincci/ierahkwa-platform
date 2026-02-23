using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.Government.Modules.CMS.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.CMS.Api
{
    internal class CMSModule : IModule
    {
        public const string BasePath = "api/cms";        
        public string Name { get; } = "CMS";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"cms"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            app.UseModuleRequests()
                .Subscribe<GetContent, ContentDto?>("cms/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetContent, ContentDto?>(query, cancellationToken))
                .Subscribe<GetContentBySlug, ContentDto?>("cms/by-slug",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetContentBySlug, ContentDto?>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
